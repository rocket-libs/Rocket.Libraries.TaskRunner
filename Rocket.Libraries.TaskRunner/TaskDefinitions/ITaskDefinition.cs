using Rocket.Libraries.TaskRunner.TaskPreconditions;
using System;
using System.Collections.Immutable;

namespace Rocket.Libraries.TaskRunner.TaskDefinitions
{
    public interface ITaskDefinition<TIdentifier> : IModelBase<TIdentifier>
    {
        TimeSpan Interval { get; set; }

        string Name { get; set; }

        Func<ImmutableList<TaskPrecondition<TIdentifier>>, ImmutableList<TaskPrecondition<TIdentifier>>> OnBeforePreconditionEvaluation { get; }
    }
}