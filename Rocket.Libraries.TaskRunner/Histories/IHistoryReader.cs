using System;
using System.Collections.Generic;
using System.Text;

namespace Rocket.Libraries.TaskRunner.Histories
{
    public interface IHistoryReader<TIdentifier> : IInstantiator<IHistory<TIdentifier>>
    {
    }
}