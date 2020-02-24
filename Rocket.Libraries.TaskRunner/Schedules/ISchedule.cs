using System;

namespace Rocket.Libraries.TaskRunner.Schedules
{
    public interface ISchedule<TIdentifier> : IModelBase<TIdentifier>
    {
        DateTime LastRun { get; set; }

        TIdentifier TaskDefinitionId { get; set; }
    }
}