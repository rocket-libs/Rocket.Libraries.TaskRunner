namespace Rocket.Libraries.TaskRunner.Performance.TaskDefinitionStates
{
    public interface ITaskDefinitionState<TIdentifier> : IModelBase<TIdentifier>
    {
        TIdentifier TaskDefinitionId { get; set; }

        bool Disabled { get; set; }
    }
}