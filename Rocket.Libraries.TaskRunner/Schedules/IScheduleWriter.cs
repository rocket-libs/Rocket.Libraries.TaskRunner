using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Rocket.Libraries.TaskRunner.Schedules
{
    public interface IScheduleWriter<TIdentifier> : IScopedServiceConsumer
    {
        Task WriteAsync(ImmutableList<ISchedule<TIdentifier>> schedules);
    }
}