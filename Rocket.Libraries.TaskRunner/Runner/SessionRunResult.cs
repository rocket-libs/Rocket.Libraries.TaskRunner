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
        public ImmutableList<History<TIdentifier>> Histories { get; set; } = ImmutableList<History<TIdentifier>>.Empty;

        public ImmutableList<Schedule<TIdentifier>> Schedules { get; set; } = ImmutableList<Schedule<TIdentifier>>.Empty;
    }
}