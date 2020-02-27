using System;
using System.Collections.Generic;
using System.Text;

namespace Rocket.Libraries.TaskRunner.Performance.FaultHandling
{
    public class CriticalFaultException : Exception
    {
        public CriticalFaultException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}