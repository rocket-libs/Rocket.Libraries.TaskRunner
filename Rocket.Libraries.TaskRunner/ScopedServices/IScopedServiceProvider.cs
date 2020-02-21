using Microsoft.Extensions.DependencyInjection;

namespace Rocket.Libraries.TaskRunner.ScopedServices
{
    public interface IScopedServiceProvider
    {
        IServiceScope Scope { get; set; }

        TService GetService<TService>();
    }
}