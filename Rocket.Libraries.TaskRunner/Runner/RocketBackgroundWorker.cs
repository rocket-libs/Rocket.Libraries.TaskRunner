using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.CircuitBreaker;
using Rocket.Libraries.TaskRunner.Configuration;
using Rocket.Libraries.TaskRunner.OnDemandQueuing;
using Rocket.Libraries.TaskRunner.Performance.FaultHandling;
using Rocket.Libraries.TaskRunner.ServiceDetails;
using Rocket.Libraries.TaskRunner.TaskDefinitions;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rocket.Libraries.TaskRunner.Runner
{
    public class RocketBackgroundWorker<TIdentifier> : BackgroundService
    {
        private static AsyncCircuitBreakerPolicy circuitBreaker;

        private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);

        private readonly IServiceScopeFactory serviceScopeFactory;

        private readonly IOnDemandQueueManager<TIdentifier> onDemandQueueManager;

        private readonly IFaultReporter<TIdentifier> faultReporter;

        private Timer timer;

        public RocketBackgroundWorker(
            IServiceScopeFactory serviceScopeFactory,
            IOnDemandQueueManager<TIdentifier> onDemandQueueManager,
            IFaultReporter<TIdentifier> faultReporter)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.onDemandQueueManager = onDemandQueueManager;
            this.faultReporter = faultReporter;
        }

        public override void Dispose()
        {
            timer?.Change(Timeout.Infinite, 0);
            timer.Dispose();
            base.Dispose();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            timer = new Timer(async (a) => await WorkAsync(), null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private async Task WorkAsync()
        {
            try
            {
                await semaphoreSlim.WaitAsync();
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    InitializePollyIfRequired(scope);
                    await circuitBreaker.ExecuteAsync(async () => await RunInCircuitAsync(scope));
                }
            }
            catch (TaskExecutionFaultException<TIdentifier> criticalException)
            {
                var taskDefinition = criticalException.TaskDefinition;
                if (taskDefinition == null)
                {
                    throw criticalException;
                }
                await faultReporter.ReportAsync(taskDefinition, criticalException.InnerException, false);
            }
            catch { }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        private async Task RunInCircuitAsync(IServiceScope scope)
        {
            var runManager =
                    scope.ServiceProvider
                        .GetRequiredService<IRunManager<TIdentifier>>();

            await runManager.RunAsync();
        }

        private void InitializePollyIfRequired(IServiceScope scope)
        {
            if (circuitBreaker == null)
            {
                ServiceDetailTracker.WriteStatus(false, DateTime.Now, "Normal start");
                var configurationProvider = scope.ServiceProvider.GetService<IRocketConfigurationProvider>();
                circuitBreaker = Policy
                        .Handle<Exception>()
                        .CircuitBreakerAsync(
                            1,
                            TimeSpan.FromMilliseconds(configurationProvider.TaskRunnerSettings.CircuitBreakerDelayMilliSeconds),
                            (exception, timeSpan, context) =>
                            {
                                onDemandQueueManager.DeQueueAll();
                                _ = timeSpan;
                                _ = context;
                                var message = $"Failure reason: '{exception.Message}'";
                                ServiceDetailTracker.WriteStatus(true, DateTime.Now, message);
                            },
                            (context) =>
                            {
                                _ = context;
                                ServiceDetailTracker.WriteStatus(false, DateTime.Now, "Resumed after failure");
                            }
                         );
            }
        }
    }
}