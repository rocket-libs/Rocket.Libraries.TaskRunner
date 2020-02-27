using Rocket.Libraries.TaskRunner.TaskDefinitions;
using System.Threading.Tasks;

namespace Rocket.Libraries.TaskRunner.Performance.TaskDefinitionStates
{
    public interface ITaskDefinitionStateWriter<TIdentifier> : IScopedServiceAccessor
    {
        Task WriteAsync(ITaskDefinitionState<TIdentifier> taskDefinitionState);
    }
}