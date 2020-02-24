using Rocket.Libraries.TaskRunner.Schedules;
using Rocket.Libraries.TaskRunner.TaskDefinitions;
using Rocket.Libraries.TaskRunner.Utility;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Rocket.Libraries.TaskRunner.Runner
{
    public class DueTasksFilter<TIdentifier> : IDueTasksFilter<TIdentifier>
    {
        private readonly IDateTimeProvider dateTimeProvider;

        public DueTasksFilter(IDateTimeProvider dateTimeProvider)
        {
            this.dateTimeProvider = dateTimeProvider;
        }

        public ImmutableList<ITaskDefinition<TIdentifier>> GetWithOnlyDueTasks(ImmutableList<ITaskDefinition<TIdentifier>> candidateTasks, ImmutableList<ISchedule<TIdentifier>> schedules)
        {
            var referenceTime = dateTimeProvider.Now;
            var dueTasks = ImmutableList<ITaskDefinition<TIdentifier>>.Empty;

            Func<ITaskDefinition<TIdentifier>, ISchedule<TIdentifier>, bool> taskIsDue = (taskDefinition, schedule) =>
            {
                var taskHasNeverBeenRun = schedule.LastRun == default;
                var timespanIsNegative = taskDefinition.Interval < TimeSpan.FromMilliseconds(0);
                var dueAt = (timespanIsNegative && taskHasNeverBeenRun) ? default : schedule.LastRun.Add(taskDefinition.Interval);
                var isDue = dueAt <= referenceTime;
                return isDue;
            };

            foreach (var singleTaskDefinition in candidateTasks)
            {
                var taskSchedule = schedules.Single(candidateSchedule => EqualityComparer<TIdentifier>.Default.Equals(singleTaskDefinition.Id, candidateSchedule.TaskDefinitionId));
                var isDue = taskIsDue(singleTaskDefinition, taskSchedule);
                if (isDue)
                {
                    dueTasks = dueTasks.Add(singleTaskDefinition);
                }
            }
            return dueTasks;
        }

        public void Dispose()
        {
        }
    }
}