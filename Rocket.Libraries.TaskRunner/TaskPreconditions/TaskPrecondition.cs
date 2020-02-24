using Rocket.Libraries.TaskRunner.TaskDefinitions;
using System;
using System.Threading.Tasks;

namespace Rocket.Libraries.TaskRunner.TaskPreconditions
{
    public abstract class TaskPrecondition<TIdentifier>
    {
        public abstract string TaskName { get; set; }

        public abstract Func<ITaskDefinition<TIdentifier>, Task<bool>> Condition { get; set; }

        public abstract string DisplayLabel { get; set; }
    }
}