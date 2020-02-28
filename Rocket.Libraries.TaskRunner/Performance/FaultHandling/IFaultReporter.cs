using Rocket.Libraries.TaskRunner.TaskDefinitions;
using System;
using System.Threading.Tasks;

namespace Rocket.Libraries.TaskRunner.Performance.FaultHandling
{
    public interface IFaultReporter<TIdentifier>
    {
        Task ReportAsync(ITaskDefinition<TIdentifier> taskDefinition, Exception exception);

        Task ReportShutdownAsync(Exception exception);
    }
}