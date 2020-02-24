using Rocket.Libraries.TaskRunner.TaskDefinitions;
using System;

namespace Rocket.Libraries.TaskRunnerTests.TaskDefinitions
{
    internal class TaskDefinition<T> : ITaskDefinition<T>
    {
        public string Name { get; set; }

        public TimeSpan Interval { get; set; }

        public T Id { get; set; }
    }
}