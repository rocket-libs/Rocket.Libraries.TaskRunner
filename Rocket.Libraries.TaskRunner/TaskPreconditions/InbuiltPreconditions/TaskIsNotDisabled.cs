using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Rocket.Libraries.TaskRunner.Performance.FaultHandling;
using Rocket.Libraries.TaskRunner.Performance.TaskDefinitionStates;
using Rocket.Libraries.TaskRunner.TaskDefinitions;

namespace Rocket.Libraries.TaskRunner.TaskPreconditions.InbuiltPreconditions
{
    public class TaskIsNotDisabled<TIdentifier> : TaskPrecondition<TIdentifier>
    {
        public override string TaskName { get; set; }

        public override Func<ITaskDefinition<TIdentifier>, Task<bool>> PassesAsync { get; set; }

        public override string DisplayLabel { get; set; }

        public static TaskIsNotDisabled<TIdentifier> GetInstance(ImmutableList<ITaskDefinitionState<TIdentifier>> taskDefinitionStates)
        {
            return new TaskIsNotDisabled<TIdentifier>
            {
                DisplayLabel = "Task Is Not Disabled",
                PassesAsync = async (taskDefinition) =>
                {
                    await Task.Run(() => { });
                    var explicitStateNotSet = taskDefinitionStates.Count == 0;
                    if (explicitStateNotSet)
                    {
                        return true;
                    }
                    else
                    {
                        var targetDefinitionStates = taskDefinitionStates.Where(a => EqualityComparer<TIdentifier>.Default.Equals(a.TaskDefinitionId, taskDefinition.Id)).ToImmutableList();
                        if (targetDefinitionStates.Count > 1)
                        {
                            var innerException = new Exception($"Expected only one task definition state for task definition with id '{taskDefinition.Id}'");
                            throw new TaskExecutionFaultException<TIdentifier>(taskDefinition, innerException);
                        }
                        else if (targetDefinitionStates.Count == 0)
                        {
                            return true;
                        }
                        else
                        {
                            return targetDefinitionStates.Single().Disabled == false;
                        }
                    }
                }
            };
        }
    }
}