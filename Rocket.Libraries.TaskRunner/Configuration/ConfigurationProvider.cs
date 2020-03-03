using Microsoft.Extensions.Options;

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