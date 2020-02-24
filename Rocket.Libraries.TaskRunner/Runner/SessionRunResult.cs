using Rocket.Libraries.TaskRunner.Histories;
using Rocket.Libraries.TaskRunner.Schedules;
using System.Collections.Immutable;

namespace Rocket.Libraries.TaskRunner.Runner
{
    public class SessionRunResult<TIdentifier>
    {
        public ImmutableList<IHistory<TIdentifier>> Histories { get; set; } = ImmutableList<IHistory<TIdentifier>>.Empty;

        public ImmutableList<ISchedule<TIdentifier>> Schedules { get; set; } = ImmutableList<ISchedule<TIdentifier>>.Empty;
    }
}