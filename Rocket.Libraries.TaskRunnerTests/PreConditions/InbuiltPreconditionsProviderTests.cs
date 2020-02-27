using Moq;
using Rocket.Libraries.TaskRunner;
using Rocket.Libraries.TaskRunner.Performance.TaskDefinitionStates;
using Rocket.Libraries.TaskRunner.TaskDefinitions;
using Rocket.Libraries.TaskRunner.TaskPreconditions;
using Rocket.Libraries.TaskRunnerTests.TaskDefinitions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rocket.Libraries.TaskRunnerTests.PreConditions
{
    public class InbuiltPreconditionsProviderTests
    {
        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public async Task DisabledTaskPreconditionWorksProperly(bool disabled, bool expectToPass)
        {
            const string taskName = "Name Of Task";
            var taskDefinitionStateReader = new Mock<ITaskDefinitionStateReader<string>>();

            var taskDefinitions = ImmutableList<ITaskDefinition<string>>.Empty
                .Add(new TaskDefinition<string>
                {
                    Name = taskName
                } as ITaskDefinition<string>);

            var taskDefinitionStates = ImmutableList<ITaskDefinitionState<string>>.Empty.Add(new TaskDefinitionState<string>
            {
                Disabled = disabled
            });

            var inbuiltTaskPreconditions = new InbuiltTaskPreconditionsProvider<string>(taskDefinitionStateReader.Object)
                    .GetInBuiltPreconditions(taskDefinitions, taskDefinitionStates);

            var passed = await inbuiltTaskPreconditions.Single().PassesAsync(taskDefinitions.Single());
            var preConditionName = inbuiltTaskPreconditions.Single().TaskName;

            Assert.Equal(expectToPass, passed);
            Assert.Equal(taskName, preConditionName);
        }
    }
}