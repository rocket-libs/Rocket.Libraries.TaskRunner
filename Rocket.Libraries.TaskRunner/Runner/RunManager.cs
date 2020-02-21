using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rocket.Libraries.TaskRunner.Conditions;
using Rocket.Libraries.TaskRunner.Histories;
using Rocket.Libraries.TaskRunner.Schedules;
using Rocket.Libraries.TaskRunner.ScopedServices;
using Rocket.Libraries.TaskRunner.TaskDefinitions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rocket.Libraries.TaskRunner.Runner
{
    public class RunManager<TIdentifier> : IRunManager<TIdentifier>
    {
        private readonly IScheduleReader<TIdentifier> scheduleReader;

        private readonly ITaskDefinitionReader<TIdentifier> taskDefinitionReader;

        private readonly IRunner<TIdentifier> runner;

        private readonly IHistoryWriter<TIdentifier> historyWriter;

        private readonly IScheduleWriter<TIdentifier> scheduleWriter;

        private readonly IPreconditionReader<TIdentifier> preconditionReader;

        private readonly IDueTasksFilter<TIdentifier> dueTasksFilter;

        private readonly IHistoryReader<TIdentifier> historyReader;

        private readonly IServiceScopeFactory serviceScopeFactory;

        private Timer timer;

        public RunManager(
            IScheduleReader<TIdentifier> scheduleReader,
            ITaskDefinitionReader<TIdentifier> taskDefinitionReader,
            IRunner<TIdentifier> runner,
            IHistoryWriter<TIdentifier> historyWriter,
            IScheduleWriter<TIdentifier> scheduleWriter,
            IPreconditionReader<TIdentifier> preconditionReader,
            IDueTasksFilter<TIdentifier> dueTasksFilter,
            IHistoryReader<TIdentifier> historyReader,
            IServiceScopeFactory serviceScopeFactory)
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
        }

        public async Task<SessionRunResult<TIdentifier>> RunAsync(IScopedServiceProvider scopedServiceProvider)
        {
            var schedules = await GetSchedulesAsync(scopedServiceProvider);
            var candidateTasks = await GetCandidateTasks(schedules, scopedServiceProvider);
            var dueTasks = GetWithOnlyDueTasks(candidateTasks, schedules);
            return await RunDueTasks(dueTasks, schedules);
        }

        private async Task<SessionRunResult<TIdentifier>> RunDueTasks(ImmutableList<ITaskDefinition<TIdentifier>> candidateTasks, ImmutableList<ISchedule<TIdentifier>> schedules)
        {
            using (runner)
            {
                using (var preconditionEvaluator = new PreconditionEvaluator<TIdentifier>())
                {
                    var sessionRunResult = new SessionRunResult<TIdentifier>();
                    var preconditions = await preconditionReader.GetByTaskNameAsync(candidateTasks.Select(a => a.Name).ToImmutableList());
                    foreach (var singleTaskDefinition in candidateTasks)
                    {
                        var startTime = DateTime.Now;
                        await RunSingleTask(singleTaskDefinition, preconditionEvaluator, schedules, preconditions, sessionRunResult, startTime);
                    }

                    await historyWriter.WriteAsync(sessionRunResult.Histories);
                    await scheduleWriter.WriteAsync(sessionRunResult.Schedules);
                    return sessionRunResult;
                }
            }
        }

        private async Task RunSingleTask(ITaskDefinition<TIdentifier> singleTaskDefinition, PreconditionEvaluator<TIdentifier> preconditionEvaluator, ImmutableList<ISchedule<TIdentifier>> schedules, ImmutableList<TaskPrecondition<TIdentifier>> preconditions, SessionRunResult<TIdentifier> sessionRunResult, DateTime startTime)
        {
            var taskSchedule = scheduleReader.GetNew();
            var history = historyReader.GetNew();
            history.StartTime = startTime;
            history.TaskDefinitionId = singleTaskDefinition.Id;

            var failingPrecondition = await preconditionEvaluator.GetFailingPrecondition(singleTaskDefinition, preconditions);
            var allPreconditionsPassed = string.IsNullOrEmpty(failingPrecondition);

            if (allPreconditionsPassed)
            {
                taskSchedule = schedules.Single(candidateSchedule => EqualityComparer<TIdentifier>.Default.Equals(candidateSchedule.TaskDefinitionId, singleTaskDefinition.Id));
                var runResult = await runner.RunAsync(singleTaskDefinition);
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
                scheduleReader.ScopedServiceProvider = scopedServiceProvider;
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
                    taskDefinitionReader.ScopedServiceProvider = scopedServiceProvider;
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
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var scopedServiceProvider = scope.ServiceProvider.GetService<IScopedServiceProvider>();
                scopedServiceProvider.Scope = scope;
                await RunAsync(scopedServiceProvider);
            }
        }
    }
}