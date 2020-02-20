using System;
using System.Collections.Generic;
using System.Text;

namespace Rocket.Libraries.TaskRunner.TaskDefinitions
{
    public class TaskDefinition<TIdentifier> : ModelBase<TIdentifier>
    {
        public string Name { get; set; }

        public string DisplayLabel { get; set; }

        public TimeSpan Interval { get; set; }
    }
}