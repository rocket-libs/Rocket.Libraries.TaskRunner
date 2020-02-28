using Rocket.Libraries.TaskRunner.TaskDefinitions;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Rocket.Libraries.TaskRunner.Performance.TaskDefinitionStates
{
    public interface ITaskDefinitionStateReader<TIdentifier> : IInstantiator<ITaskDefinitionState<TIdentifier>>, IScopedServiceAccessor
    {
        Task<ImmutableList<ITaskDefinitionState<TIdentifier>>> GetByTaskDefinitionIds(ImmutableList<TIdentifier> taskDefinitionIds);
    }
}