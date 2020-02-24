using Rocket.Libraries.TaskRunner.TaskDefinitions;
using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Rocket.Libraries.TaskRunner.Histories
{
    public interface IHistoryWriter<TIdentifier> : IDisposable, IScopedServiceAccessor
    {
        Task WriteAsync(ImmutableList<IHistory<TIdentifier>> histories);
    }
}