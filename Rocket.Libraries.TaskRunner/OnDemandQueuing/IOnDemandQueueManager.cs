using System.Collections.Immutable;

namespace Rocket.Libraries.TaskRunner.OnDemandQueuing
{
    public interface IOnDemandQueueManager<TIdentifier>
    {
        ImmutableList<TIdentifier> Queued { get; }

        ImmutableList<Schedules.ISchedule<TIdentifier>> GetOnDemandSchedulesFlagged(ImmutableList<Schedules.ISchedule<TIdentifier>> schedules, ImmutableList<TIdentifier> onDemandTaskDefinitionIds);

        void Queue(TIdentifier taskDefinitionId);

        void DeQueue(TIdentifier taskDefinitionId);

        void DeQueueAll();
    }
}