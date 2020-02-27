namespace Rocket.Libraries.TaskRunner.Configuration
{
    public interface IConfigurationProvider
    {
        TaskRunnerSettings TaskRunnerSettings { get; }
    }
}