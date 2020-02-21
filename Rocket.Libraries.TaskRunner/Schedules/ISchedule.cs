using System;
using System.Collections.Generic;
using System.Text;

namespace Rocket.Libraries.TaskRunner.Schedules
{
    public interface ISchedule<TIdentifier> : IModelBase<TIdentifier>
    {
        DateTime LastRun { get; set; }

        TIdentifier TaskDefinitionId { get; set; }
    }
}