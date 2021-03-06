﻿using Rocket.Libraries.TaskRunner.Performance.TaskDefinitionStates;
using Rocket.Libraries.TaskRunner.TaskDefinitions;
using System.Collections.Immutable;

namespace Rocket.Libraries.TaskRunner.TaskPreconditions
{
    public interface IInbuiltTaskPreconditionsProvider<TIdentifier>
    {
        ImmutableList<TaskPrecondition<TIdentifier>> GetInBuiltPreconditions(ImmutableList<ITaskDefinition<TIdentifier>> taskDefinitions, ImmutableList<ITaskDefinitionState<TIdentifier>> taskDefinitionStates);
    }
}