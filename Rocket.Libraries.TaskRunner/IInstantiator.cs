using System;
using System.Collections.Generic;
using System.Text;

namespace Rocket.Libraries.TaskRunner
{
    public interface IInstantiator<T>
    {
        T GetNew();
    }
}