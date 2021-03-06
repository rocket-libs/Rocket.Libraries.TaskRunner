﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Rocket.Libraries.TaskRunner.Logging
{
    public interface ITaskRunnerLogger
    {
        Guid RunId { get; set; }

        void LogInformation(string message);

        void LogError(string message);

        void LogException(string message, Exception exception);

        void LogException(Exception exception);
    }
}