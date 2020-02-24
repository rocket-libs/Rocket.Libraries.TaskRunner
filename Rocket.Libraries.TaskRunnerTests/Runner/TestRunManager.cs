using Microsoft.Extensions.DependencyInjection;
using Rocket.Libraries.TaskRunner.Histories;
using Rocket.Libraries.TaskRunner.Runner;
using Rocket.Libraries.TaskRunner.Schedules;
using Rocket.Libraries.TaskRunner.ScopedServices;
using Rocket.Libraries.TaskRunner.TaskDefinitions;
using Rocket.Libraries.TaskRunner.TaskPreconditions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Libraries.TaskRunnerTests.Runner
{
    internal class TestRunManager : RunManager<Guid>
    {
        public TestRunManager(IScheduleReader<Guid> scheduleReader, ITaskDefinitionReader<Guid> taskDefinitionReader, IRunner<Guid> runner, IHistoryWriter<Guid> historyWriter, IScheduleWriter<Guid> scheduleWriter, IPreconditionReader<Guid> preconditionReader, IDueTasksFilter<Guid> dueTasksFilter, IHistoryReader<Guid> historyReader, IServiceScopeFactory serviceScopeFactory) : base(scheduleReader, taskDefinitionReader, runner, historyWriter, scheduleWriter, preconditionReader, dueTasksFilter, historyReader, serviceScopeFactory)
        {
        }

        public override Task OnRunCompletedAsync(bool succeeded, IScopedServiceProvider scopedServiceProvider, SessionRunResult<Guid> sessionRunResult)
        {
            return Task.CompletedTask;
        }
    }
}