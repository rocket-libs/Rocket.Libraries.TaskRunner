using Rocket.Libraries.TaskRunner.Schedules;
using Rocket.Libraries.TaskRunner.TaskDefinitions;
using Rocket.Libraries.TaskRunner.Utility;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Rocket.Libraries.TaskRunner.Runner
{
    public class DueTasksFilter<TIdentifier> : IDueTasksFilter<TIdentifier>
    {
        private readonly IDateTimeProvider dateTimeProvider;

        public DueTasksFilter(IDateTimeProvider dateTimeProvider)
        {
            this.dateTimeProvider = dateTimeProvider;
        }

        public ImmutableList<TaskDefinition<TIdentifier>> GetWithOnlyDueTasks(ImmutableList<TaskDefinition<TIdentifier>> candidateTasks, ImmutableList<Schedule<TIdentifier>> schedules)
        {
            var referenceTime = dateTimeProvider.Now;
            var dueTasks = ImmutableList<TaskDefinition<TIdentifier>>.Empty;

            Func<TaskDefinition<TIdentifier>, Schedule<TIdentifier>, bool> taskIsDue = (taskDefinition, schedule) =>
        {
            var dueAt = schedule.LastRun.Add(taskDefinition.Interval);
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