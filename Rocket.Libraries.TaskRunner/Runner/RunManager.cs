using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Rocket.Libraries.TaskRunner.Configuration;
using Rocket.Libraries.TaskRunner.Histories;
using Rocket.Libraries.TaskRunner.Performance.FaultHandling;
using Rocket.Libraries.TaskRunner.Performance.TaskDefinitionStates;
using Rocket.Libraries.TaskRunner.Schedules;
using Rocket.Libraries.TaskRunner.ScopedServices;
using Rocket.Libraries.TaskRunner.ServiceDetails;
using Rocket.Libraries.TaskRunner.TaskDefinitions;
using Rocket.Libraries.TaskRunner.TaskPreconditions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Rocket.Libraries.TaskRunner.Runner
{
    public class RunManager<TIdentifier> : IRunManager<TIdentifier>
    {
        private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);

        private readonly IScheduleReader<TIdentifier> scheduleReader;

        private readonly ITaskDefinitionReader<TIdentifier> taskDefinitionReader;

        private readonly IRunner<TIdentifier> runner;

        private readonly IHistoryWriter<TIdentifier> historyWriter;

        private readonly IScheduleWriter<TIdentifier> scheduleWriter;

        private readonly IPreconditionReader<TIdentifier> preconditionReader;

        private readonly IDueTasksFilter<TIdentifier> dueTasksFilter;

        private readonly IHistoryReader<TIdentifier> historyReader;

        private readonly IServiceScopeFactory serviceScopeFactory;

        private readonly IInbuiltTaskPreconditionsProvider<TIdentifier> inbuiltTaskPreconditionsProvider;

        private readonly ITaskDefinitionStateReader<TIdentifier> taskDefinitionStateReader;

        private readonly ITaskDefinitionStateWriter<TIdentifier> taskDefinitionStateWriter;

        private readonly IFaultHandler<TIdentifier> faultHandler;

        private readonly IFaultReporter<TIdentifier> faultReporter;

        private static AsyncCircuitBreakerPolicy circuitBreaker;

        private Timer timer;

        public ILogger<RunManager<TIdentifier>> Logger { get; }

        public RunManager(
            IScheduleReader<TIdentifier> scheduleReader,
            ITaskDefinitionReader<TIdentifier> taskDefinitionReader,
            IRunner<TIdentifier> runner,
            IHistoryWriter<TIdentifier> historyWriter,
            IScheduleWriter<TIdentifier> scheduleWriter,
            IPreconditionReader<TIdentifier> preconditionReader,
            IDueTasksFilter<TIdentifier> dueTasksFilter,
            IHistoryReader<TIdentifier> historyReader,
            IServiceScopeFactory serviceScopeFactory,
            ILogger<RunManager<TIdentifier>> logger,
            IInbuiltTaskPreconditionsProvider<TIdentifier> inbuiltTaskPreconditionsProvider,
            ITaskDefinitionStateReader<TIdentifier> taskDefinitionStateReader,
            ITaskDefinitionStateWriter<TIdentifier> taskDefinitionStateWriter,
            IFaultHandler<TIdentifier> faultHandler,
            IFaultReporter<TIdentifier> faultReporter)
        {
            this.scheduleReader = scheduleReader;
            this.taskDefinitionReader = taskDefinitionReader;
            this.runner = runner;
            this.historyWriter = historyWriter;
            this.scheduleWriter = scheduleWriter;
            this.preconditionReader = preconditionReader;
            this.dueTasksFilter = dueTasksFilter;
            this.historyReader = historyReader;
            this.serviceScopeFactory = serviceScopeFactory;
            Logger = logger;
            this.inbuiltTaskPreconditionsProvider = inbuiltTaskPreconditionsProvider;
            this.taskDefinitionStateReader = taskDefinitionStateReader;
            this.taskDefinitionStateWriter = taskDefinitionStateWriter;
            this.faultHandler = faultHandler;
            this.faultReporter = faultReporter;
        }

        public async Task RunAsync(IScopedServiceProvider scopedServiceProvider)
        {
            try
            {
                await OnRunStartAsync();
                var schedules = await GetSchedulesAsync(scopedServiceProvider);
                var candidateTasks = await GetCandidateTasks(schedules, scopedServiceProvider);
                var dueTasks = GetWithOnlyDueTasks(candidateTasks, schedules);
                var result = await RunDueTasks(scopedServiceProvider, dueTasks, schedules);
                await OnRunCompletedAsync(true, scopedServiceProvider, result, null);
            }
            catch (Exception e)
            {
                await OnRunCompletedAsync(false, scopedServiceProvider, null, e);
                throw;
            }
        }

        public virtual Task OnRunStartAsync()
        {
            return Task.CompletedTask;
        }

        public virtual Task OnRunCompletedAsync(bool succeeded, IScopedServiceProvider scopedServiceProvider, SessionRunResult<TIdentifier> sessionRunResult, Exception exception)
        {
            _ = succeeded;
            _ = scopedServiceProvider;
            _ = sessionRunResult;
            _ = exception;
            return Task.CompletedTask;
        }

        private async Task<SessionRunResult<TIdentifier>> RunDueTasks(IScopedServiceProvider scopedServiceProvider, ImmutableList<ITaskDefinition<TIdentifier>> candidateTasks, ImmutableList<ISchedule<TIdentifier>> schedules)
        {
            using (runner)
            {
                using (var preconditionEvaluator = new PreconditionEvaluator<TIdentifier>())
                {
                    var taskDefinitionStates = await taskDefinitionStateReader.GetByTaskDefinitionIds(candidateTasks.Select(a => a.Id).ToImmutableList());
                    var sessionRunResult = new SessionRunResult<TIdentifier>();
                    var preconditions = await preconditionReader.GetByTaskNameAsync(candidateTasks.Select(a => a.Name).ToImmutableList());
                    var inBuiltPreconditions = inbuiltTaskPreconditionsProvider.GetInBuiltPreconditions(candidateTasks, taskDefinitionStates);
                    preconditions = preconditions.AddRange(inBuiltPreconditions);
                    foreach (var singleTaskDefinition in candidateTasks)
                    {
                        var startTime = DateTime.Now;
                        await RunSingleTask(scopedServiceProvider, singleTaskDefinition, preconditionEvaluator, schedules, preconditions, sessionRunResult, startTime);
                    }

                    await historyWriter.WriteAsync(sessionRunResult.Histories);
                    await scheduleWriter.WriteAsync(sessionRunResult.Schedules);
                    return sessionRunResult;
                }
            }
        }

        private async Task RunSingleTask(IScopedServiceProvider scopedServiceProvider, ITaskDefinition<TIdentifier> singleTaskDefinition, PreconditionEvaluator<TIdentifier> preconditionEvaluator, ImmutableList<ISchedule<TIdentifier>> schedules, ImmutableList<TaskPrecondition<TIdentifier>> preconditions, SessionRunResult<TIdentifier> sessionRunResult, DateTime startTime)
        {
            var taskSchedule = scheduleReader.GetNew();
            var history = historyReader.GetNew();
            history.StartTime = startTime;
            history.TaskDefinitionId = singleTaskDefinition.Id;
            var runResult = default(SingleTaskRunResult);

            var failingPrecondition = await preconditionEvaluator.GetFailingPrecondition(singleTaskDefinition, preconditions);
            var allPreconditionsPassed = string.IsNullOrEmpty(failingPrecondition);

            if (allPreconditionsPassed)
            {
                taskSchedule = schedules.Single(candidateSchedule => EqualityComparer<TIdentifier>.Default.Equals(candidateSchedule.TaskDefinitionId, singleTaskDefinition.Id));
                try
                {
                    runResult = await runner.RunAsync(scopedServiceProvider, singleTaskDefinition);
                }
                catch (Exception e)
                {
                    await faultHandler.HandleAsync(singleTaskDefinition, e);
                    runResult = new SingleTaskRunResult
                    {
                        Succeeded = false,
                        Remarks = e.Message,
                    };
                }
                history.Remarks = runResult.Remarks;
                history.Status = runResult.Succeeded ? RunHistoryStatuses.CompletedSuccessfully : RunHistoryStatuses.Failed;
            }
            else
            {
                history.Remarks = $"Precondition '{failingPrecondition}' failed";
                history.Status = RunHistoryStatuses.PreconditionPreventedRun;
            }
            history.EndTime = taskSchedule.LastRun = DateTime.Now;

            sessionRunResult.Histories = sessionRunResult.Histories.Add(history);
            sessionRunResult.Schedules = sessionRunResult.Schedules.Add(taskSchedule);
        }

        private async Task<ImmutableList<ISchedule<TIdentifier>>> GetSchedulesAsync(IScopedServiceProvider scopedServiceProvider)
        {
            using (scheduleReader)
            {
                return await scheduleReader.GetAllAsync();
            }
        }

        private async Task<ImmutableList<ITaskDefinition<TIdentifier>>> GetCandidateTasks(ImmutableList<ISchedule<TIdentifier>> schedules, IScopedServiceProvider scopedServiceProvider)
        {
            var noScheduledTasks = schedules == null || schedules.Count == 0;
            if (noScheduledTasks)
            {
                return null;
            }
            else
            {
                var ids = schedules.Select(singleSchedule => singleSchedule.TaskDefinitionId).ToImmutableList();
                using (taskDefinitionReader)
                {
                    return await taskDefinitionReader.GetByIdsAsync(ids);
                }
            }
        }

        private ImmutableList<ITaskDefinition<TIdentifier>> GetWithOnlyDueTasks(ImmutableList<ITaskDefinition<TIdentifier>> candidateTasks, ImmutableList<ISchedule<TIdentifier>> schedules)
        {
            using (dueTasksFilter)
            {
                return dueTasksFilter.GetWithOnlyDueTasks(candidateTasks, schedules);
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(async (a) => await WorkAsync(), null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }

        private async Task WorkAsync()
        {
            try
            {
                await semaphoreSlim.WaitAsync();
                InitializePollyIfRequired();
                await circuitBreaker
                .ExecuteAsync(() => RunInCircuitAsync());
            }
            catch { }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        private async Task RunInCircuitAsync()
        {
            try
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    /*var scopedServiceProvider = scope.ServiceProvider.GetService<IScopedServiceProvider>();
                    scopedServiceProvider.Scope = scope;
                    SetupScopedServiceReader(scopedServiceProvider);
                    await RunAsync(scopedServiceProvider);*
                }
            }
            catch (Exception e)
            {
                await faultReporter.ReportShutdownAsync(e);
                throw new CriticalFaultException("Error occured in a point during which it is not possible to determine errant task.", e);
                throw;
            }
        }

        private void InitializePollyIfRequired()
        {
            if (circuitBreaker == null)
            {
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    ServiceDetailTracker.WriteStatus(false, DateTime.Now, "Normal start");
                    var configurationProvider = scope.ServiceProvider.GetService<IConfigurationProvider>();
                    circuitBreaker = Policy
                            .Handle<Exception>()
                            .CircuitBreakerAsync(
                                1,
                                TimeSpan.FromMilliseconds(configurationProvider.TaskRunnerSettings.CircuitBreakerDelayMilliSeconds),
                                (exception, timeSpan, context) =>
                                {
                                    _ = timeSpan;
                                    _ = context;
                                    var message = $"Failure reason: '{exception.Message}'";
                                    ServiceDetailTracker.WriteStatus(false, DateTime.Now, message);
                                },
                                (context) =>
                                {
                                    _ = context;
                                    ServiceDetailTracker.WriteStatus(true, DateTime.Now, "Resumed after failure");
                                }
                             );
                }
            }
        }

        /*private void SetupScopedServiceReader(IScopedServiceProvider scopedServiceProvider)
        {
            taskDefinitionReader.ScopedServiceProvider = scopedServiceProvider;
            scheduleReader.ScopedServiceProvider = scopedServiceProvider;
            scheduleWriter.ScopedServiceProvider = scopedServiceProvider;
            historyReader.ScopedServiceProvider = scopedServiceProvider;
            historyWriter.ScopedServiceProvider = scopedServiceProvider;
            taskDefinitionStateReader.ScopedServiceProvider = scopedServiceProvider;
            taskDefinitionStateWriter.ScopedServiceProvider = scopedServiceProvider;
        }*/
                }
            }