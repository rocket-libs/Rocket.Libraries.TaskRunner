using System;

namespace Rocket.Libraries.TaskRunner.Performance
{
    internal interface IPerformanceContract<TIdentifier> : IModelBase<TIdentifier>
    {
        TIdentifier TaskDefinitionId { get; set; }

        TimeSpan TimeoutDuration { get; set; }
    }
}