namespace Rocket.Libraries.TaskRunner.Configuration
{
    public interface IRocketConfigurationProvider
    {
        TaskRunnerSettings TaskRunnerSettings { get; }
    }
}