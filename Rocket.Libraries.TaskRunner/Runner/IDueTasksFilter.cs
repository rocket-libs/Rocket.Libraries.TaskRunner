using System;
using System.Collections.Immutable;
using Rocket.Libraries.TaskRunner.Schedules;
using Rocket.Libraries.TaskRunner.TaskDefinitions;

namespace Rocket.Libraries.TaskRunner.Runner
{
    public interface IDueTasksFilter<TIdentifier> : IDisposable
    {
        ImmutableList<ITaskDefinition<TIdentifier>> GetWithOnlyDueTasks(ImmutableList<ITaskDefinition<TIdentifier>> candidateTasks, ImmutableList<ISchedule<TIdentifier>> schedules);
    }
}