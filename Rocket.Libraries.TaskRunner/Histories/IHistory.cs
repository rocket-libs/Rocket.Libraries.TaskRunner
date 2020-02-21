using System;
using System.Collections.Generic;
using System.Text;

namespace Rocket.Libraries.TaskRunner.Histories
{
    public interface IHistory<TIdentifier> : IModelBase<TIdentifier>
    {
        TIdentifier TaskDefinitionId { get; set; }

        DateTime StartTime { get; set; }

        DateTime EndTime { get; set; }

        string Status { get; set; }

        string Remarks { get; set; }
    }
}