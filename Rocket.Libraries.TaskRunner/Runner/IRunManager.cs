using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Rocket.Libraries.TaskRunner.Conditions;

namespace Rocket.Libraries.TaskRunner.Runner
{
    public interface IRunManager<TIdentifier> : IHostedService, IDisposable
    {
        Task<SessionRunResult<TIdentifier>> RunAsync();
    }
}