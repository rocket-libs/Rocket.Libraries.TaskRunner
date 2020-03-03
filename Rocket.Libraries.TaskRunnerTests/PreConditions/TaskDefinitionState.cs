using Rocket.Libraries.TaskRunner.Performance.TaskDefinitionStates;

namespace Rocket.Libraries.TaskRunnerTests.PreConditions
{
    internal class TaskDefinitionState<T> : ITaskDefinitionState<T>
    {
        public T TaskDefinitionId { get; set; }

        public bool Disabled { get; set; }

        public T Id { get; set; }
    }
}