using System;
using System.Collections.Generic;
using System.Text;

namespace Rocket.Libraries.TaskRunner.Histories
{
    public class History<TIdentifier> : ModelBase<TIdentifier>
    {
        public TIdentifier TaskDefinitionId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string Status { get; set; }

        public string Remarks { get; set; }
    }
}