using Rocket.Libraries.TaskRunner.TaskDefinitions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Libraries.TaskRunner.Conditions
{
    public class TaskPrecondition<TIdentifier>
    {
        public string TaskName { get; set; }

        public Func<ITaskDefinition<TIdentifier>, Task<bool>> Condition { get; set; }

        public string DisplayLabel { get; set; }
    }
}