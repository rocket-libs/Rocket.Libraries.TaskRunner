using Rocket.Libraries.TaskRunner.ScopedServices;

namespace Rocket.Libraries.TaskRunner.TaskDefinitions
{
    public interface IScopedServiceAccessor
    {
        IScopedServiceProvider ScopedServiceProvider { get; set; }
    }
}