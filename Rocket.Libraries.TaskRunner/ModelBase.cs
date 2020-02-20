using System;
using System.Collections.Generic;
using System.Text;

namespace Rocket.Libraries.TaskRunner
{
    public abstract class ModelBase<TIdentifier>
    {
        public TIdentifier Id { get; set; }
    }
}