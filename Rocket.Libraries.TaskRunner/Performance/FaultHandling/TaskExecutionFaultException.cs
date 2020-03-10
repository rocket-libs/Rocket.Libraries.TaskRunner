using Rocket.Libraries.TaskRunner.TaskDefinitions;
using System;

namespace Rocket.Libraries.TaskRunner.Performance.FaultHandling
{
    public class TaskExecutionFaultException<TIdentifier> : Exception
    {
        public TaskExecutionFaultException(TIdentifier taskDefinitionId, Exception innerException)
            : base("Error occured")
        {
            TaskDefinitionId = taskDefinitionId;
        }

        public TaskExecutionFaultException(ITaskDefinition<TIdentifier> taskDefinition, Exception innerException)
            : base("Error occured", innerException)
        {
            TaskDefinition = taskDefinition;
            TaskDefinitionId = TaskDefinition.Id;
        }

        public ITaskDefinition<TIdentifier> TaskDefinition { get; }

        public TIdentifier TaskDefinitionId { get; }
    }
}