using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Libraries.TaskRunner.Schedules
{
    public interface IScheduleReader<TIdentifier> : IDisposable
    {
        Task<ImmutableList<Schedule<TIdentifier>>> GetAllAsync();
    }
}