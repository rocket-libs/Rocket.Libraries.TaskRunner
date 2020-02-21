using Rocket.Libraries.TaskRunner.Histories;
using Rocket.Libraries.TaskRunner.Schedules;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Rocket.Libraries.TaskRunner.Runner
{
    public class SessionRunResult<TIdentifier>
    {
        public ImmutableList<IHistory<TIdentifier>> Histories { get; set; } = ImmutableList<IHistory<TIdentifier>>.Empty;

        public ImmutableList<ISchedule<TIdentifier>> Schedules { get; set; } = ImmutableList<ISchedule<TIdentifier>>.Empty;
    }
}