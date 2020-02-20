using Moq;
using Rocket.Libraries.TaskRunner.Conditions;
using Rocket.Libraries.TaskRunner.Histories;
using Rocket.Libraries.TaskRunner.Runner;
using Rocket.Libraries.TaskRunner.Schedules;
using Rocket.Libraries.TaskRunner.TaskDefinitions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rocket.Libraries.TaskRunnerTests.Runner
{
    public class RunManagerTests
    {
        [Fact]
        public async Task TaskIsRunSuccessfully()
        {
            var taskDefinitionId = Guid.NewGuid();
            var scheduleReader = new Mock<IScheduleReader<Guid>>();
            var taskDefinitionReader = new Mock<ITaskDefinitionReader<Guid>>();
            var runner = new Mock<IRunner<Guid>>();
            var historyWriter = new Mock<IHistoryWriter<Guid>>();
            var scheduleWriter = new Mock<IScheduleWriter<Guid>>();
            var preconditionReader = new Mock<IPreconditionReader<Guid>>();
            var dueTasksFilter = new Mock<IDueTasksFilter<Guid>>();
            var targetTaskDefinitionList = ImmutableList<TaskDefinition<Guid>>.Empty.Add(new TaskDefinition<Guid>
            {
                Id = taskDefinitionId
            });

            scheduleReader.Setup(a => a.GetAllAsync())
                .ReturnsAsync(ImmutableList<Schedule<Guid>>.Empty.Add(new Schedule<Guid>
                {
                    LastRun = DateTime.Now.AddDays(-1),
                    TaskDefinitionId = taskDefinitionId,
                }));

            taskDefinitionReader.Setup(a => a.GetByIdsAsync(It.IsAny<ImmutableList<Guid>>()))
                .ReturnsAsync(targetTaskDefinitionList);

            runner.Setup(a => a.RunAsync(It.IsAny<TaskDefinition<Guid>>()))
                .ReturnsAsync(new SingleTaskRunResult
                {
                    Remarks = "Blah",
                    Succeeded = true
                });

            dueTasksFilter.Setup(a => a.GetWithOnlyDueTasks(It.IsAny<ImmutableList<TaskDefinition<Guid>>>(), It.IsAny<ImmutableList<Schedule<Guid>>>()))
                .Returns(targetTaskDefinitionList);

            var runManager = new RunManager<Guid>(
                scheduleReader.Object,
                taskDefinitionReader.Object,
                runner.Object,
                historyWriter.Object,
                scheduleWriter.Object,
                preconditionReader.Object,
                dueTasksFilter.Object
                );

            var result = await runManager.RunAsync();
            Assert.True(result.Histories != null);
            Assert.True(result.Histories.First().Status == RunHistoryStatuses.CompletedSuccessfully);
        }
    }
}
