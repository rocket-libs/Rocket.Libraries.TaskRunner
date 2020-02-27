using Microsoft.Extensions.DependencyInjection;
using Polly;
using Rocket.Libraries.TaskRunner.Performance.FaultHandling;
using Rocket.Libraries.TaskRunner.Runner;
using Rocket.Libraries.TaskRunner.ScopedServices;
using Rocket.Libraries.TaskRunner.TaskPreconditions;
using Rocket.Libraries.TaskRunner.Utility;
using System;

namespace Rocket.Libraries.TaskRunner
{
    public static class ServiceRegistrationHelper
    {
        public static IServiceCollection RegisterInbuiltServices<TIdentifier>(this IServiceCollection services)
        {
            services
                .AddScoped<IScopedServiceProvider, ScopedServiceProvider>()
                .AddTransient<IDueTasksFilter<TIdentifier>, DueTasksFilter<TIdentifier>>()
                .AddTransient<IDateTimeProvider, DateTimeProvider>()
                .AddTransient<IInbuiltTaskPreconditionsProvider<TIdentifier>, InbuiltTaskPreconditionsProvider<TIdentifier>>()
                .AddTransient<IFaultHandler<TIdentifier>, FaultHandler<TIdentifier>>();
            return services;
        }
    }
}