using System;

namespace Rocket.Libraries.TaskRunner.Utility
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime Now => DateTime.Now;

        public void Dispose()
        {
        }
    }
}