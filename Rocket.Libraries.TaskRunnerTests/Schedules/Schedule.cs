using Rocket.Libraries.TaskRunner.Schedules;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rocket.Libraries.TaskRunnerTests.Schedules
{
    internal class Schedule<T> : ISchedule<T>
    {
        public DateTime LastRun { get; set; }

        public T TaskDefinitionId { get; set; }

        public T Id { get; set; }
    }
}