using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Libraries.TaskRunner.Histories
{
    public interface IHistoryWriter<TIdentifier> : IDisposable
    {
        Task WriteAsync(ImmutableList<IHistory<TIdentifier>> histories);
    }
}