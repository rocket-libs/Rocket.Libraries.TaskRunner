using System;
using System.Collections.Generic;
using System.Text;

namespace Rocket.Libraries.TaskRunner
{
    public interface IModelBase<TIdentifier>
    {
        TIdentifier Id { get; set; }
    }
}