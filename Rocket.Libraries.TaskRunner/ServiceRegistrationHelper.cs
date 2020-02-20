using Microsoft.Extensions.DependencyInjection;
using Rocket.Libraries.TaskRunner.Runner;
using Rocket.Libraries.TaskRunner.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rocket.Libraries.TaskRunner
{
    public static class ServiceRegistrationHelper
    {
        public static IServiceCollection RegisterInbuiltTaskRunnerServices<TIdentifier>(this IServiceCollection services)
        {
            services
                .AddTransient<IDueTasksFilter<TIdentifier>, DueTasksFilter<TIdentifier>>()
                .AddTransient<IDateTimeProvider, DateTimeProvider>();
            return services;
        }
    }
}