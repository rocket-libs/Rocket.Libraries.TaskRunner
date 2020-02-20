using System;
using System.Collections.Generic;
using System.Text;

namespace Rocket.Libraries.TaskRunner.Schedules
{
    public class Schedule<TIdentifier> : ModelBase<TIdentifier>
    {
        public DateTime LastRun { get; set; }

        public TIdentifier TaskDefinitionId { get; set; }
    }
}