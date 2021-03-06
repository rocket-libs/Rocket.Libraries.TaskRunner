﻿using Moq;
using Rocket.Libraries.TaskRunner.Logging;
using Rocket.Libraries.TaskRunner.TaskPreconditions;
using Rocket.Libraries.TaskRunnerTests.TaskDefinitions;
using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Xunit;

namespace Rocket.Libraries.TaskRunnerTests.PreConditions
{
    public class PreconditionEvaluatorTests
    {
        [Theory]
        [InlineData(false, false)]
        [InlineData(true, true)]
        public async Task PreConditionsFilterCorrectly(bool preconditionResult, bool errorMessageShouldBeEmpty)
        {
            var taskRunnerLogger = new Mock<ITaskRunnerLogger>();
            const string taskName = "TaskName";

            var taskDefinition = new TaskDefinition<Guid>
            {
                Id = Guid.NewGuid(),
                Name = taskName
            };

            var preconditions = ImmutableList<TaskPrecondition<Guid>>.Empty.Add(new DummyPrecondition<Guid>
            {
                PassesAsync = async (a) => await Task.Run(() => preconditionResult),
                DisplayLabel = "Blah",
                TaskName = taskName,
            });

            var preconditionEvaluator = new PreconditionEvaluator<Guid>(taskRunnerLogger.Object);
            var result = await preconditionEvaluator.GetFailingPrecondition(taskDefinition, preconditions);
            var errorMessageIsEmpty = string.IsNullOrEmpty(result);
            Assert.Equal(errorMessageShouldBeEmpty, errorMessageIsEmpty);
        }
    }
}