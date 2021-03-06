﻿using Moq;
using Rocket.Libraries.TaskRunner.Runner;
using Rocket.Libraries.TaskRunner.Schedules;
using Rocket.Libraries.TaskRunner.TaskDefinitions;
using Rocket.Libraries.TaskRunner.Utility;
using Rocket.Libraries.TaskRunnerTests.Schedules;
using Rocket.Libraries.TaskRunnerTests.TaskDefinitions;
using System;
using System.Collections.Immutable;
using System.Linq;
using Xunit;

namespace Rocket.Libraries.TaskRunnerTests.Runner
{
    public class DueTasksFilterTests
    {
        [Theory]
        [InlineData(-1, true)]
        [InlineData(0, true)]
        [InlineData(1, false)]
        public void DueDatesAreFilteredCorrectly(int milliSecondsAway, bool isDue)
        {
            const int taskDefinitionId = 1;
            var datetimeProvider = new Mock<IDateTimeProvider>();
            var referenceDate = DateTime.Now;
            datetimeProvider.Setup(a => a.Now)
                .Returns(referenceDate);

            var candidateTasks = ImmutableList<ITaskDefinition<long>>.Empty
                .Add(new TaskDefinition<long>
                {
                    Interval = TimeSpan.FromMilliseconds(0),
                    Id = taskDefinitionId
                });

            var schedules = ImmutableList<ISchedule<long>>.Empty
                .Add(new Schedule<long>
                {
                    LastRun = referenceDate.AddMilliseconds(milliSecondsAway),
                    TaskDefinitionId = taskDefinitionId
                });

            var dueTasksFilter = new DueTasksFilter<long>(datetimeProvider.Object);
            var result = dueTasksFilter.GetWithOnlyDueTasks(candidateTasks, schedules);

            var evaluatedAsDue = result.Any(a => a.Id == taskDefinitionId);

            Assert.Equal(isDue, evaluatedAsDue);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, true)]
        public void NeverRunTasksAreQueue(bool hasBeenRunPreviously, bool expectedToBeQueued)
        {
            const int taskDefinitionId = 1;
            const int intervalMilliseconds = -1;
            var datetimeProvider = new Mock<IDateTimeProvider>();

            datetimeProvider.Setup(a => a.Now)
                .Returns(DateTime.Now);

            var referenceDate = datetimeProvider.Object.Now;

            var candidateTasks = ImmutableList<ITaskDefinition<long>>.Empty
                .Add(new TaskDefinition<long>
                {
                    Interval = TimeSpan.FromMilliseconds(intervalMilliseconds),
                    Id = taskDefinitionId
                });

            var schedules = ImmutableList<ISchedule<long>>.Empty
                .Add(new Schedule<long>
                {
                    LastRun = hasBeenRunPreviously ? referenceDate.AddMilliseconds(intervalMilliseconds * -1) : default,
                    TaskDefinitionId = taskDefinitionId
                });

            var dueTasksFilter = new DueTasksFilter<long>(datetimeProvider.Object);
            var result = dueTasksFilter.GetWithOnlyDueTasks(candidateTasks, schedules);

            var wasQueued = result.Count > 0;
            Assert.Equal(expectedToBeQueued, wasQueued);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(false, false)]
        public void OnDemandFlagIsRespected(bool isOnDemand, bool expectedToBeQueued)
        {
            const int taskDefinitionId = 1;
            const int intervalMilliseconds = 1000 * 60;
            var datetimeProvider = new Mock<IDateTimeProvider>();

            datetimeProvider.Setup(a => a.Now)
                .Returns(DateTime.Now);

            var candidateTasks = ImmutableList<ITaskDefinition<long>>.Empty
                .Add(new TaskDefinition<long>
                {
                    Interval = TimeSpan.FromMilliseconds(intervalMilliseconds),
                    Id = taskDefinitionId
                });

            var schedules = ImmutableList<ISchedule<long>>.Empty
                .Add(new Schedule<long>
                {
                    LastRun = datetimeProvider.Object.Now,
                    TaskDefinitionId = taskDefinitionId,
                    IsOnDemand = isOnDemand
                });

            var dueTasksFilter = new DueTasksFilter<long>(datetimeProvider.Object);
            var result = dueTasksFilter.GetWithOnlyDueTasks(candidateTasks, schedules);

            var wasQueued = result.Count > 0;
            Assert.Equal(expectedToBeQueued, wasQueued);
        }
    }
}