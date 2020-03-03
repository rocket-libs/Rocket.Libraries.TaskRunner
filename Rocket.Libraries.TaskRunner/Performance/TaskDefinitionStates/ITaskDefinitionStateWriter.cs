using System.Threading.Tasks;

namespace Rocket.Libraries.TaskRunner.Performance.TaskDefinitionStates
{
    public interface ITaskDefinitionStateWriter<TIdentifier> : IScopedServiceConsumer
    {
        Task WriteAsync(ITaskDefinitionState<TIdentifier> taskDefinitionState);
    }
}