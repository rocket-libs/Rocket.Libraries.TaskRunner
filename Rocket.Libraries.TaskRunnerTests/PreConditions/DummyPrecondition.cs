using Rocket.Libraries.TaskRunner.TaskDefinitions;
using Rocket.Libraries.TaskRunner.TaskPreconditions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Libraries.TaskRunnerTests.PreConditions
{
    internal class DummyPrecondition<TIdenitifier> : TaskPrecondition<TIdenitifier>
    {
        public override Func<ITaskDefinition<TIdenitifier>, Task<bool>> Condition { get; set; }

        public override string DisplayLabel { get; set; }

        public override string TaskName { get; set; }
    }
}