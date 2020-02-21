using Rocket.Libraries.TaskRunner.ScopedServices;
using Rocket.Libraries.TaskRunner.TaskDefinitions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Libraries.TaskRunner.Schedules
{
    public interface IScheduleReader<TIdentifier> : IDisposable, IInstantiator<ISchedule<TIdentifier>>, ITaskRunnerService
    {
        Task<ImmutableList<ISchedule<TIdentifier>>> GetAllAsync();
    }
}