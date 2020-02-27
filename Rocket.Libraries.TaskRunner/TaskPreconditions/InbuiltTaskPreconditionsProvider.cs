using Rocket.Libraries.TaskRunner.Performance.TaskDefinitionStates;
using Rocket.Libraries.TaskRunner.TaskDefinitions;
using Rocket.Libraries.TaskRunner.TaskPreconditions.InbuiltPreconditions;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Rocket.Libraries.TaskRunner.TaskPreconditions
{
    public class InbuiltTaskPreconditionsProvider<TIdentifier> : IInbuiltTaskPreconditionsProvider<TIdentifier>
    {
        private readonly ITaskDefinitionStateReader<TIdentifier> taskDefinitionStateReader;

        public InbuiltTaskPreconditionsProvider(
            ITaskDefinitionStateReader<TIdentifier> taskDefinitionStateReader)
        {
            this.taskDefinitionStateReader = taskDefinitionStateReader;
        }

        public ImmutableList<TaskPrecondition<TIdentifier>> GetInBuiltPreconditions(ImmutableList<ITaskDefinition<TIdentifier>> taskDefinitions, ImmutableList<ITaskDefinitionState<TIdentifier>> taskDefinitionStates)
        {
            var result = ImmutableList<TaskPrecondition<TIdentifier>>.Empty;
            foreach (var singleTaskDefinition in taskDefinitions)
            {
                var taskSpecificPreconditions = ImmutableList<TaskPrecondition<TIdentifier>>.Empty
                     .Add(TaskIsNotDisabled<TIdentifier>.GetInstance(taskDefinitionStates));

                foreach (var singleTaskPrecondition in taskSpecificPreconditions)
                {
                    singleTaskPrecondition.TaskName = singleTaskDefinition.Name;
                }

                result = result.AddRange(taskSpecificPreconditions);
            }

            return result;
        }
    }
}