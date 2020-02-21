using Rocket.Libraries.TaskRunner.Conditions;
using Rocket.Libraries.TaskRunner.TaskDefinitions;
using Rocket.Libraries.TaskRunnerTests.TaskDefinitions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Rocket.Libraries.TaskRunnerTests.Conditions
{
    public class PreconditionEvaluatorTests
    {
        [Theory]
        [InlineData(false, false)]
        [InlineData(true, true)]
        public async Task PreConditionsFilterCorrectly(bool preconditionResult, bool errorMessageShouldBeEmpty)
        {
            var taskDefinition = new TaskDefinition<Guid>
            {
                Id = Guid.NewGuid(),
                Name = "Some Name"
            };

            var preconditions = ImmutableList<TaskPrecondition<Guid>>.Empty.Add(new TaskPrecondition<Guid>
            {
                Condition = async (a) => await Task.Run(() => preconditionResult),
                DisplayLabel = "Blah",
                TaskName = taskDefinition.Name
            });

            var preconditionEvaluator = new PreconditionEvaluator<Guid>();
            var result = await preconditionEvaluator.GetFailingPrecondition(taskDefinition, preconditions);
            var errorMessageIsEmpty = string.IsNullOrEmpty(result);
            Assert.Equal(errorMessageShouldBeEmpty, errorMessageIsEmpty);
        }
    }
}