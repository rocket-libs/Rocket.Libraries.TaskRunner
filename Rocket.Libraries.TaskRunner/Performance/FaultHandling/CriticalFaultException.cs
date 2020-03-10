using System;

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