using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocket.Libraries.TaskRunner.Histories;
using Rocket.Libraries.TaskRunner.Logging;
using Rocket.Libraries.TaskRunner.OnDemandQueuing;
using Rocket.Libraries.TaskRunner.Performance.FaultHandling;
using Rocket.Libraries.TaskRunner.Performance.TaskDefinitionStates;
using Rocket.Libraries.TaskRunner.Schedules;
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

        private readonly IDueTasksFilter<TIdentifier> dueTasksFilter;

        private readonly IFaultHandler<TIdentifier> faultHandler;

        private readonly IFaultReporter<TIdentifier> faultReporter;

        private readonly IHistoryReader<TIdentifier> historyReader;

        private readonly IHistoryWriter<TIdentifier> historyWriter;

        private readonly IInbuiltTaskPreconditionsProvider<TIdentifier> inbuiltTaskPreconditionsProvider;

        private readonly IOnDemandQueueManager<TIdentifier> onDemandQueueManager;

        private readonly IPreconditionReader<TIdentifier> preconditionReader;

        private readonly IRunner<TIdentifier> runner;

        private readonly IScheduleReader<TIdentifier> scheduleReader;

        private readonly IScheduleWriter<TIdentifier> scheduleWriter;

        private readonly IServiceScopeFactory serviceScopeFactory;

        private readonly ITaskDefinitionReader<TIdentifier> taskDefinitionReader;

        private readonly ITaskDefinitionStateReader<TIdentifier> taskDefinitionStateReader;

        private readonly ITaskDefinitionStateWriter<TIdentifier> taskDefinitionStateWriter;

        private readonly ITaskRunnerLogger taskRunnerLogger;

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
            IFaultReporter<TIdentifier> faultReporter,
            IOnDemandQueueManager<TIdentifier> onDemandQueueManager,
            ITaskRunnerLogger taskRunnerLogger)
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
            this.onDemandQueueManager = onDemandQueueManager;
            this.taskRunnerLogger = taskRunnerLogger;
        }

        public ILogger<RunManager<TIdentifier>> Logger { get; }

        public void Dispose()
        {
        }

        public virtual Task OnRunCompletedAsync(bool succeeded, SessionRunResult<TIdentifier> sessionRunResult, Exception exception)
        {
            _ = succeeded;
            _ = sessionRunResult;
            _ = exception;
            if (succeeded)
            {
                taskRunnerLogger.LogInformation($"Run completed without exception.");
            }
            else
            {
                taskRunnerLogger.LogError($"Error(s) occured during run");
            }

            return Task.CompletedTask;
        }

        public virtual Task OnRunStartAsync()
        {
            return Task.CompletedTask;
        }

        public async Task RunAsync()
        {
            try
            {
                var runId = Guid.NewGuid();
                taskRunnerLogger.RunId = runId;
                taskRunnerLogger.LogInformation("Starting run");
                await OnRunStartAsync();
                var schedules = await GetSchedulesAsync();
                taskRunnerLogger.LogInformation($"Found {schedules.Count} schedules");
                var candidateTasks = await GetCandidateTasks(schedules);
                taskRunnerLogger.LogInformation($"Found {candidateTasks.Count} tasks total");
                var dueTasks = GetWithOnlyDueTasks(candidateTasks, schedules);
                taskRunnerLogger.LogInformation($"Found {dueTasks.Count} due tasks");
                var result = await RunDueTasks(dueTasks, schedules, runId);
                await OnRunCompletedAsync(true, result, null);
            }
            catch (Exception e)
            {
                taskRunnerLogger.LogException(e);
                await OnRunCompletedAsync(false, null, e);
                throw;
            }
        }

        private async Task<ImmutableList<ITaskDefinition<TIdentifier>>> GetCandidateTasks(ImmutableList<ISchedule<TIdentifier>> schedules)
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

        private async Task<ImmutableList<ISchedule<TIdentifier>>> GetSchedulesAsync()
        {
            using (scheduleReader)
            {
                var queuedTaskDefinitionIds = onDemandQueueManager.Queued; // This caching is important so we are sure we work with the same set of task definition ids within this scope.
                var schedules = await scheduleReader.GetAllAsync(queuedTaskDefinitionIds);
                return onDemandQueueManager.GetOnDemandSchedulesFlagged(schedules, queuedTaskDefinitionIds);
            }
        }

        private ImmutableList<ITaskDefinition<TIdentifier>> GetWithOnlyDueTasks(ImmutableList<ITaskDefinition<TIdentifier>> candidateTasks, ImmutableList<ISchedule<TIdentifier>> schedules)
        {
            using (dueTasksFilter)
            {
                return dueTasksFilter.GetWithOnlyDueTasks(candidateTasks, schedules);
            }
        }

        private async Task<SessionRunResult<TIdentifier>> RunDueTasks(ImmutableList<ITaskDefinition<TIdentifier>> candidateTasks, ImmutableList<ISchedule<TIdentifier>> schedules, Guid runId)
        {
            using (runner)
            {
                using (var preconditionEvaluator = new PreconditionEvaluator<TIdentifier>(taskRunnerLogger))
                {
                    var taskDefinitionStates = await taskDefinitionStateReader.GetByTaskDefinitionIds(candidateTasks.Select(a => a.Id).ToImmutableList());
                    var sessionRunResult = new SessionRunResult<TIdentifier>();
                    var preconditions = await preconditionReader.GetByTaskNameAsync(candidateTasks.Select(a => a.Name).ToImmutableList());
                    var inBuiltPreconditions = inbuiltTaskPreconditionsProvider.GetInBuiltPreconditions(candidateTasks, taskDefinitionStates);
                    preconditions = preconditions.AddRange(inBuiltPreconditions);
                    preconditions = preconditions.GroupBy(a => new { t = a.GetType(), n = a.TaskName })
                        .Select(a => a.First())
                        .ToImmutableList();
                    foreach (var singleTaskDefinition in candidateTasks)
                    {
                        var startTime = DateTime.Now;
                        await RunSingleTask(singleTaskDefinition, preconditionEvaluator, schedules, preconditions, sessionRunResult, startTime, runId);
                    }

                    await historyWriter.WriteAsync(sessionRunResult.Histories);
                    await scheduleWriter.WriteAsync(sessionRunResult.Schedules);
                    return sessionRunResult;
                }
            }
        }

        private async Task RunSingleTask(ITaskDefinition<TIdentifier> singleTaskDefinition, PreconditionEvaluator<TIdentifier> preconditionEvaluator, ImmutableList<ISchedule<TIdentifier>> schedules, ImmutableList<TaskPrecondition<TIdentifier>> preconditions, SessionRunResult<TIdentifier> sessionRunResult, DateTime startTime, Guid runId)
        {
            var taskSchedule = schedules.Single(candidateSchedule => EqualityComparer<TIdentifier>.Default.Equals(candidateSchedule.TaskDefinitionId, singleTaskDefinition.Id));
            var history = historyReader.GetNew();
            history.StartTime = startTime;
            history.TaskDefinitionId = singleTaskDefinition.Id;
            history.RunId = runId;
            var runResult = default(SingleTaskRunResult);

            var failingPrecondition = await preconditionEvaluator.GetFailingPrecondition(singleTaskDefinition, preconditions);
            var allPreconditionsPassed = string.IsNullOrEmpty(failingPrecondition);

            if (allPreconditionsPassed)
            {
                try
                {
                    runResult = await runner.RunAsync(singleTaskDefinition);
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
                finally
                {
                    if (taskSchedule.IsOnDemand)
                    {
                        onDemandQueueManager.DeQueue(taskSchedule.TaskDefinitionId);
                    }
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
    }
}