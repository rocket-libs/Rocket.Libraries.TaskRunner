using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Rocket.Libraries.TaskRunner.TaskDefinitions
{
    public interface ITaskDefinitionReader<TIdentifier> : IDisposable, IInstantiator<ITaskDefinition<TIdentifier>>
    {
        Task<ImmutableList<ITaskDefinition<TIdentifier>>> GetByIdsAsync(ImmutableList<TIdentifier> ids);
    }
}