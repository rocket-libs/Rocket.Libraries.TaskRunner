using Rocket.Libraries.TaskRunner.Histories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rocket.Libraries.TaskRunnerTests.Histories
{
    internal class History<T> : IHistory<T>
    {
        public T TaskDefinitionId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string Status { get; set; }

        public string Remarks { get; set; }

        public T Id { get; set; }
    }
}