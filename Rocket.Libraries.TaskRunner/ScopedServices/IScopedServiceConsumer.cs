namespace Rocket.Libraries.TaskRunner.ScopedServices
{
    public interface IScopedServiceConsumer
    {
        IScopedServiceProvider ScopedServiceProvider { get; set; }
    }
}