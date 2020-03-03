using Microsoft.Extensions.DependencyInjection;
using Rocket.Libraries.TaskRunner.Configuration;
using Rocket.Libraries.TaskRunner.Performance.FaultHandling;
using Rocket.Libraries.TaskRunner.Runner;
using Rocket.Libraries.TaskRunner.ScopedServices;
using Rocket.Libraries.TaskRunner.TaskPreconditions;
using Rocket.Libraries.TaskRunner.Utility;

namespace Rocket.Libraries.TaskRunner
{
    public static class ServiceRegistrationHelper
    {
        internal static string TaskRunnerSettingsName = "TaskRunnerSettings";

        public static IServiceCollection RegisterInbuiltServices<TIdentifier>(this IServiceCollection services, TaskRunnerSettings taskRunnerSettings)
        {
            services
                .AddScoped<IScopedServiceProvider, ScopedServiceProvider>()
                .AddScoped<IConfigurationProvider, ConfigurationProvider>()
                .AddTransient<IDueTasksFilter<TIdentifier>, DueTasksFilter<TIdentifier>>()
                .AddTransient<IDateTimeProvider, DateTimeProvider>()
                .AddTransient<IInbuiltTaskPreconditionsProvider<TIdentifier>, InbuiltTaskPreconditionsProvider<TIdentifier>>()
                .AddTransient<IFaultHandler<TIdentifier>, FaultHandler<TIdentifier>>();
            services.Configure<TaskRunnerSettings>(TaskRunnerSettingsName, a =>
            {
                a.CircuitBreakerDelayMilliSeconds = taskRunnerSettings.CircuitBreakerDelayMilliSeconds;
            });
            return services;
        }
    }
}