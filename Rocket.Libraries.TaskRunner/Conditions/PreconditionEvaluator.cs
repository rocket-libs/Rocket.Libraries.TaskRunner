using Rocket.Libraries.TaskRunner.TaskDefinitions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Libraries.TaskRunner.Conditions
{
    public class PreconditionEvaluator<TIdentifier> : IDisposable
    {
        public void Dispose()
        {
        }

        public async Task<string> GetFailingPrecondition(TaskDefinition<TIdentifier> taskDefinition, ImmutableList<TaskPrecondition<TIdentifier>> preconditions)
        {
            if (preconditions == null || preconditions.Count == 0)
            {
                return string.Empty;
            }
            else
            {
                return await GetEvaluationResult(taskDefinition, preconditions);
            }
        }

        private async Task<string> GetEvaluationResult(TaskDefinition<TIdentifier> taskDefinition, ImmutableList<TaskPrecondition<TIdentifier>> preconditions)
        {
            var taskPreconditions = preconditions.Where(candidatePrecondition => candidatePrecondition.TaskName.Equals(taskDefinition.Name, StringComparison.InvariantCulture))
                        .ToImmutableList();
            var taskHasNoPreConditions = taskPreconditions.Count == 0;
            if (taskHasNoPreConditions)
            {
                return string.Empty;
            }
            else
            {
                return await GetFirstFailingPreconditionAsync(taskDefinition, taskPreconditions);
            }
        }

        private async Task<string> GetFirstFailingPreconditionAsync(TaskDefinition<TIdentifier> taskDefinition, ImmutableList<TaskPrecondition<TIdentifier>> taskPreconditions)
        {
            foreach (var singlePrecondition in taskPreconditions)
            {
                var passed = await singlePrecondition.Condition(taskDefinition);
                if (!passed)
                {
                    return singlePrecondition.DisplayLabel;
                }
            }
            return string.Empty;
        }
    }
}