using System;

namespace Rocket.Libraries.TaskRunner.Utility
{
    public interface IDateTimeProvider : IDisposable
    {
        DateTime Now { get; }
    }
}