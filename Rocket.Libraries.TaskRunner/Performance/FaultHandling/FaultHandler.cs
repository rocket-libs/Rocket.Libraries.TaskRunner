using Rocket.Libraries.TaskRunner.Performance.TaskDefinitionStates;
using Rocket.Libraries.TaskRunner.TaskDefinitions;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Rocket.Libraries.TaskRunner.Performance.FaultHandling
{
    internal class FaultHandler<TIdentifier> : IFaultHandler<TIdentifier>
    {
        private readonly ITaskDefinitionStateWriter<TIdentifier> taskDefinitionStateWriter;

        private readonly ITaskDefinitionStateReader<TIdentifier> taskDefinitionStateReader;

        private readonly IFaultReporter<TIdentifier> faultReporter;

        public FaultHandler(
            ITaskDefinitionStateWriter<TIdentifier> taskDefinitionStateWriter,
            ITaskDefinitionStateReader<TIdentifier> taskDefinitionStateReader,
            IFaultReporter<TIdentifier> faultReporter)
        {
            this.taskDefinitionStateWriter = taskDefinitionStateWriter;
            this.taskDefinitionStateReader = taskDefinitionStateReader;
            this.faultReporter = faultReporter;
        }

        public async Task HandleAsync(ITaskDefinition<TIdentifier> taskDefinition, Exception exception)
        {
            var taskDefinitionState = await GetTaskDefinitionState(taskDefinition.Id);
            taskDefinitionState.Disabled = true;
            await taskDefinitionStateWriter.WriteAsync(taskDefinitionState);
            await faultReporter.ReportAsync(taskDefinition, exception, true);
        }

        private async Task<ITaskDefinitionState<TIdentifier>> GetTaskDefinitionState(TIdentifier taskDefinitionId)
        {
            var taskDefinitionStates = await taskDefinitionStateReader.GetByTaskDefinitionIds(ImmutableList<TIdentifier>.Empty.Add(taskDefinitionId));
            var containsTaskDefinition = taskDefinitionStates.Count > 0;
            if (containsTaskDefinition)
            {
                return taskDefinitionStates.Single();
            }
            else
            {
                var taskDefinitionState = taskDefinitionStateReader.GetNew();
                taskDefinitionState.TaskDefinitionId = taskDefinitionId;
                return taskDefinitionState;
            }
        }
    }
}