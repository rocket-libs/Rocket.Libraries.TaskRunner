using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Libraries.TaskRunner.Conditions
{
    public interface IPreconditionReader<TIdentifier>
    {
        Task<ImmutableList<TaskPrecondition<TIdentifier>>> GetByTaskNameAsync(ImmutableList<string> taskNames);
    }
}