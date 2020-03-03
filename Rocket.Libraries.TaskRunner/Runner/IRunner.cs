using Rocket.Libraries.TaskRunner.TaskDefinitions;
using System;
using System.Threading.Tasks;

namespace Rocket.Libraries.TaskRunner.Runner
{
    public interface IRunner<TIdentifier> : IDisposable
    {
        Task<SingleTaskRunResult> RunAsync(ITaskDefinition<TIdentifier> taskDefinition);
    }
}