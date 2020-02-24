using System;

namespace Rocket.Libraries.TaskRunner.TaskDefinitions
{
    public interface ITaskDefinition<TIdentifier> : IModelBase<TIdentifier>
    {
        string Name { get; set; }

        TimeSpan Interval { get; set; }
    }
}