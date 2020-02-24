namespace Rocket.Libraries.TaskRunner
{
    public interface IInstantiator<T>
    {
        T GetNew();
    }
}