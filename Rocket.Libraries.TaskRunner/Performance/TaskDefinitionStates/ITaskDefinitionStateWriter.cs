using System.Threading.Tasks;

namespace Rocket.Libraries.TaskRunner.Performance.TaskDefinitionStates
{
    public interface ITaskDefinitionStateWriter<TIdentifier>
    {
        Task WriteAsync(ITaskDefinitionState<TIdentifier> taskDefinitionState);
    }
}