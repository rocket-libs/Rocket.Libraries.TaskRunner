using Rocket.Libraries.TaskRunner.Utility;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Libraries.TaskRunner.TaskDefinitions
{
    public interface ITaskDefinitionReader<TIdentifier> : IDisposable, IInstantiator<ITaskDefinition<TIdentifier>>, ITaskRunnerService
    {
        Task<ImmutableList<ITaskDefinition<TIdentifier>>> GetByIdsAsync(ImmutableList<TIdentifier> ids);
    }
}