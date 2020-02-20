using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Libraries.TaskRunner.Schedules
{
    public interface IScheduleWriter<TIdentifier>
    {
        Task WriteAsync(ImmutableList<Schedule<TIdentifier>> schedules);
    }
}