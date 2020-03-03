namespace Rocket.Libraries.TaskRunner.Histories
{
    public interface IHistoryReader<TIdentifier> : IInstantiator<IHistory<TIdentifier>>, IScopedServiceConsumer
    {
    }
}