using Rocket.Libraries.TaskRunner.TaskDefinitions;

namespace Rocket.Libraries.TaskRunner.Histories
{
    public interface IHistoryReader<TIdentifier> : IInstantiator<IHistory<TIdentifier>>, IScopedServiceAccessor
    {
    }
}