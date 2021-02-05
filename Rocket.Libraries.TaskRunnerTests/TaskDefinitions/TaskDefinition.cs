using Rocket.Libraries.TaskRunner.TaskDefinitions;
using Rocket.Libraries.TaskRunner.TaskPreconditions;
using System;
using System.Collections.Immutable;

namespace Rocket.Libraries.TaskRunnerTests.TaskDefinitions
{
    internal class TaskDefinition<T> : ITaskDefinition<T>
    {
        public T Id { get; set; }

        public TimeSpan Interval { get; set; }

        public string Name { get; set; }

        public Func<ImmutableList<TaskPrecondition<T>>, ImmutableList<TaskPrecondition<T>>> OnBeforePreconditionEvaluation { get; }
    }
}