using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Rocket.Libraries.TaskRunner.ScopedServices;

namespace Rocket.Libraries.TaskRunner.Runner
{
    public interface IRunManager<TIdentifier> : IHostedService, IDisposable
    {
        Task RunAsync(IScopedServiceProvider scopedServiceProvider);
    }
}