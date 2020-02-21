using System;
using System.Collections.Generic;
using System.Text;

namespace Rocket.Libraries.TaskRunner.TaskDefinitions
{
    public interface ITaskDefinition<TIdentifier> : IModelBase<TIdentifier>
    {
        string Name { get; set; }

        TimeSpan Interval { get; set; }
    }
}