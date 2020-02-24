namespace Rocket.Libraries.TaskRunner
{
    public interface IModelBase<TIdentifier>
    {
        TIdentifier Id { get; set; }
    }
}