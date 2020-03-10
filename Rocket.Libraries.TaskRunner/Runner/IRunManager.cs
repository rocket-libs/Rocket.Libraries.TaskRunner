using System;
using System.Threading.Tasks;

namespace Rocket.Libraries.TaskRunner.Runner
{
    public interface IRunManager<TIdentifier> : IDisposable
    {
        Task RunAsync();
    }
}