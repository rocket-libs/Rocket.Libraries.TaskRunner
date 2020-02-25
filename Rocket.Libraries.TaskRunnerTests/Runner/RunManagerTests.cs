using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Rocket.Libraries.TaskRunner.Histories;
using Rocket.Libraries.TaskRunner.Runner;
using Rocket.Libraries.TaskRunner.Schedules;
using Rocket.Libraries.TaskRunner.ScopedServices;
using Rocket.Libraries.TaskRunner.TaskDefinitions;
using Rocket.Libraries.TaskRunner.TaskPreconditions;
using Rocket.Libraries.TaskRunnerTests.Histories;
using Rocket.Libraries.TaskRunnerTests.Schedules;
using Rocket.Libraries.TaskRunnerTests.TaskDefinitions;
using System;
using System.Collections.Immutable;
using System.Linq;
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
            var historyReader = new Mock<IHistoryReader<Guid>>();
            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            var scopedServiceProvider = new Mock<IScopedServiceProvider>();
            var logger = new Mock<Logger<TestRunManager>>();

            var targetTaskDefinitionList = ImmutableList<ITaskDefinition<Guid>>.Empty.Add(new TaskDefinition<Guid>
            {
                Id = taskDefinitionId
            });

            scheduleReader.Setup(a => a.GetAllAsync())
                .ReturnsAsync(ImmutableList<ISchedule<Guid>>.Empty.Add(new Schedule<Guid>
                {
                    LastRun = DateTime.Now.AddDays(-1),
                    TaskDefinitionId = taskDefinitionId,
                }));

            taskDefinitionReader.Setup(a => a.GetByIdsAsync(It.IsAny<ImmutableList<Guid>>()))
                .ReturnsAsync(targetTaskDefinitionList);

            runner.Setup(a => a.RunAsync(It.IsAny<IScopedServiceProvider>(), It.IsAny<ITaskDefinition<Guid>>()))
                .ReturnsAsync(new SingleTaskRunResult
                {
                    Remarks = "Blah",
                    Succeeded = true
                });

            dueTasksFilter.Setup(a => a.GetWithOnlyDueTasks(It.IsAny<ImmutableList<ITaskDefinition<Guid>>>(), It.IsAny<ImmutableList<ISchedule<Guid>>>()))
                .Returns(targetTaskDefinitionList);

            historyReader.Setup(a => a.GetNew())
                .Returns(new History<Guid>());

            var runManager = new TestRunManager(
                scheduleReader.Object,
                taskDefinitionReader.Object,
                runner.Object,
                historyWriter.Object,
                scheduleWriter.Object,
                preconditionReader.Object,
                dueTasksFilter.Object,
                historyReader.Object,
                serviceScopeFactory.Object,
                 logger.Object
                );
            await runManager.RunAsync(new ScopedServiceProvider());
            Assert.True(runManager.SessionRunResult.Histories != null);
            Assert.True(runManager.SessionRunResult.Histories.First().Status == RunHistoryStatuses.CompletedSuccessfully);
        }
    }
}