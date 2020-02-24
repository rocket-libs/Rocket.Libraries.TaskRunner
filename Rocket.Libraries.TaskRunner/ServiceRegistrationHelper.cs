using Microsoft.Extensions.DependencyInjection;
using Rocket.Libraries.TaskRunner.Runner;
using Rocket.Libraries.TaskRunner.ScopedServices;
using Rocket.Libraries.TaskRunner.Utility;

namespace Rocket.Libraries.TaskRunner
{
    public static class ServiceRegistrationHelper
    {
        public static IServiceCollection RegisterInbuiltTaskRunnerServices<TIdentifier>(this IServiceCollection services)
        {
            services
                .AddScoped<IScopedServiceProvider, ScopedServiceProvider>()
                .AddTransient<IDueTasksFilter<TIdentifier>, DueTasksFilter<TIdentifier>>()
                .AddTransient<IDateTimeProvider, DateTimeProvider>();
            return services;
        }
    }
}