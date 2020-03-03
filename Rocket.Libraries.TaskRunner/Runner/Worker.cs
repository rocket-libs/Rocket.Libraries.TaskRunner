using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rocket.Libraries.TaskRunner.Runner
{
    public class Worker<TIdentifier> : BackgroundService
    {
        private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);

        private readonly IServiceScopeFactory serviceScopeFactory;

        public Worker(IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var runManager =
                    scope.ServiceProvider
                        .GetRequiredService<IRunManager<TIdentifier>>();

                await scopedProcessingService.DoWork(stoppingToken);
            }
        }
    }
}