using Microsoft.Extensions.Options;

namespace Rocket.Libraries.TaskRunner.Configuration
{
    public class RocketConfigurationProvider : IRocketConfigurationProvider
    {
        public TaskRunnerSettings TaskRunnerSettings { get; }

        public RocketConfigurationProvider(IOptionsSnapshot<TaskRunnerSettings> taskRunnerSettings)
        {
            TaskRunnerSettings = taskRunnerSettings.Get(ServiceRegistrationHelper.TaskRunnerSettingsName);
        }
    }
}