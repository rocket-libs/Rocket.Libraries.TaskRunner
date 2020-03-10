using Rocket.Libraries.TaskRunner;
using Rocket.Libraries.TaskRunner.Performance.FaultHandling;
using Rocket.Libraries.TaskRunner.Performance.TaskDefinitionStates;
using Rocket.Libraries.TaskRunner.TaskDefinitions;
using Rocket.Libraries.TaskRunner.TaskPreconditions.InbuiltPreconditions;
using Rocket.Libraries.TaskRunnerTests.TaskDefinitions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rocket.Libraries.TaskRunnerTests.PreConditions.InbuiltPreconditions
{
    public class TaskIsNotDisabledTests
    {
        [Theory]
        [InlineData(0, false)]
        [InlineData(1, false)]
        [InlineData(2, true)]
        public async Task TaskIsNotDisabledHandlesTaskDefinitionStateCountsProperly(int stateCount, bool throwsException)
        {
            const int taskDefinitionId = 8;
            var taskDefinitionStates = ImmutableList<TaskDefinitionState<int>>.Empty;
            for (int i = 0; i < stateCount; i++)
            {
                taskDefinitionStates = taskDefinitionStates.Add(new TaskDefinitionState<int>
                {
                    TaskDefinitionId = taskDefinitionId,
                });
            }
            var taskIsNotDisabled = TaskIsNotDisabled<int>.GetInstance(taskDefinitionStates.Transform<TaskDefinitionState<int>, ITaskDefinitionState<int>>());

            var taskDefinition = new TaskDefinition<int>
            {
                Id = taskDefinitionId
            } as ITaskDefinition<int>;

            if (throwsException)
            {
                await Assert.ThrowsAsync<TaskExecutionFaultException<int>>(async () => await taskIsNotDisabled.PassesAsync(taskDefinition));
            }
            else
            {
                await taskIsNotDisabled.PassesAsync(taskDefinition);
            }
        }
    }
}