using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Rocket.Libraries.TaskRunner.TaskPreconditions
{
    public interface IPreconditionReader<TIdentifier>
    {
        Task<ImmutableList<TaskPrecondition<TIdentifier>>> GetByTaskNameAsync(ImmutableList<string> taskNames);
    }
}