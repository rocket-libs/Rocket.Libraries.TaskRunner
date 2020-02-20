using System;
using System.Collections.Immutable;
using Rocket.Libraries.TaskRunner.Schedules;
using Rocket.Libraries.TaskRunner.TaskDefinitions;

namespace Rocket.Libraries.TaskRunner.Runner
{
    public interface IDueTasksFilter<TIdentifier> : IDisposable
    {
        ImmutableList<TaskDefinition<TIdentifier>> GetWithOnlyDueTasks(ImmutableList<TaskDefinition<TIdentifier>> candidateTasks, ImmutableList<Schedule<TIdentifier>> schedules);
    }
}