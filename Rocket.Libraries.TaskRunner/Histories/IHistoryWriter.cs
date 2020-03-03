using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Rocket.Libraries.TaskRunner.Histories
{
    public interface IHistoryWriter<TIdentifier> : IDisposable, IScopedServiceConsumer
    {
        Task WriteAsync(ImmutableList<IHistory<TIdentifier>> histories);
    }
}