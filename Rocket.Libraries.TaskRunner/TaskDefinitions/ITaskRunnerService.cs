using Rocket.Libraries.TaskRunner.ScopedServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rocket.Libraries.TaskRunner.TaskDefinitions
{
    public interface ITaskRunnerService
    {
        IScopedServiceProvider ScopedServiceProvider { get; set; }
    }
}