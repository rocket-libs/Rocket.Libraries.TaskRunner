using Rocket.Libraries.TaskRunner.TaskDefinitions;
using System;

namespace Rocket.Libraries.TaskRunner.Performance.FaultHandling
{
    public class TaskExecutionFaultException<TIdentifier> : Exception
    {
        public TaskExecutionFaultException(ITaskDefinition<TIdentifier> taskDefinition, Exception innerException)
            : base("Error occured", innerException)
        {
            TaskDefinition = taskDefinition;
        }

        public ITaskDefinition<TIdentifier> TaskDefinition { get; }
    }
}