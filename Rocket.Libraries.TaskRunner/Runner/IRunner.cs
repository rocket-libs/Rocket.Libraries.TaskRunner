using Rocket.Libraries.TaskRunner.ScopedServices;
using Rocket.Libraries.TaskRunner.TaskDefinitions;
using System;
using System.Threading.Tasks;

namespace Rocket.Libraries.TaskRunner.Runner
{
    public interface IRunner<TIdentifier> : IDisposable
    {
        Task<SingleTaskRunResult> RunAsync(IScopedServiceProvider scopedServiceProvider, ITaskDefinition<TIdentifier> taskDefinition);
    }
}