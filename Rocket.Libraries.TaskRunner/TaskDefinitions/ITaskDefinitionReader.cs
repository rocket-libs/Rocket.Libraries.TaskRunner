using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Libraries.TaskRunner.TaskDefinitions
{
    public interface ITaskDefinitionReader<TIdentifier> : IDisposable
    {
        Task<ImmutableList<TaskDefinition<TIdentifier>>> GetByIdsAsync(ImmutableList<TIdentifier> ids);
    }
}