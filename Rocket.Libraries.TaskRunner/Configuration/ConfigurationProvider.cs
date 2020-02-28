using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rocket.Libraries.TaskRunner.Configuration
{
    public class ConfigurationProvider : IConfigurationProvider
    {
        public TaskRunnerSettings TaskRunnerSettings { get; }

        public ConfigurationProvider(IOptionsSnapshot<TaskRunnerSettings> taskRunnerSettings)
        {
            TaskRunnerSettings = taskRunnerSettings.Get(ServiceRegistrationHelper.TaskRunnerSettingsName);
        }
    }
}