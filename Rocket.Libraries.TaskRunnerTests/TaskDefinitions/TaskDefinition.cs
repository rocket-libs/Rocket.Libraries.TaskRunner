using Rocket.Libraries.TaskRunner.TaskDefinitions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rocket.Libraries.TaskRunnerTests.TaskDefinitions
{
    internal class TaskDefinition<T> : ITaskDefinition<T>
    {
        public string Name { get; set; }

        public TimeSpan Interval { get; set; }

        public T Id { get; set; }
    }
}