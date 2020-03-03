using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Rocket.Libraries.TaskRunner.Schedules
{
    public interface IScheduleWriter<TIdentifier>
    {
        Task WriteAsync(ImmutableList<ISchedule<TIdentifier>> schedules);
    }
}