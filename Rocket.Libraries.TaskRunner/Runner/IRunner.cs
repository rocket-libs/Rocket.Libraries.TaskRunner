using Rocket.Libraries.TaskRunner.TaskDefinitions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Libraries.TaskRunner.Runner
{
    public interface IRunner<TIdentifier> : IDisposable
    {
        Task<SingleTaskRunResult> RunAsync(TaskDefinition<TIdentifier> taskDefinition);
    }
}