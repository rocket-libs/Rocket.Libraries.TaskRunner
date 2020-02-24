using Rocket.Libraries.TaskRunner.TaskDefinitions;
using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Rocket.Libraries.TaskRunner.Schedules
{
    public interface IScheduleReader<TIdentifier> : IDisposable, IInstantiator<ISchedule<TIdentifier>>, IScopedServiceAccessor
    {
        Task<ImmutableList<ISchedule<TIdentifier>>> GetAllAsync();
    }
}