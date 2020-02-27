using Rocket.Libraries.TaskRunner.TaskDefinitions;
using System;
using System.Threading.Tasks;

namespace Rocket.Libraries.TaskRunner.Performance.FaultHandling
{
    public interface IFaultHandler<TIdentifier>
    {
        Task HandleAsync(ITaskDefinition<TIdentifier> taskDefinition, Exception exception);
    }
}