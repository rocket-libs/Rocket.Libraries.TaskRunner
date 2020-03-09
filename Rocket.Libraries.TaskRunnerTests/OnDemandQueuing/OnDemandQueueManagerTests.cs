using Rocket.Libraries.TaskRunner;
using Rocket.Libraries.TaskRunner.OnDemandQueuing;
using Rocket.Libraries.TaskRunner.Schedules;
using Rocket.Libraries.TaskRunnerTests.Schedules;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Xunit;

namespace Rocket.Libraries.TaskRunnerTests.OnDemandQueuing
{
    public class OnDemandQueueManagerTests
    {
        [Fact]
        public void OnDemandSchedulesTaggedCorrectly()
        {
            const int onDemandId = 9;
            const int notOnDemandId = 4;

            var onDemandQueueManager = new OnDemandQueueManager<int>();
            var schedules = ImmutableList<Schedule<int>>.Empty
                .Add(new Schedule<int>
                {
                    TaskDefinitionId = onDemandId
                })
                .Add(new Schedule<int>
                {
                    TaskDefinitionId = notOnDemandId,
                });
            var onDemandTaskDefinitionIds = ImmutableList<int>.Empty.Add(onDemandId);

            var processedSchedules = onDemandQueueManager.GetOnDemandSchedulesFlagged(schedules.Transform<Schedule<int>, ISchedule<int>>(), onDemandTaskDefinitionIds);

            Assert.True(processedSchedules.First(a => a.TaskDefinitionId == onDemandId).IsOnDemand);
            Assert.False(processedSchedules.First(a => a.TaskDefinitionId == notOnDemandId).IsOnDemand);
        }
    }
}