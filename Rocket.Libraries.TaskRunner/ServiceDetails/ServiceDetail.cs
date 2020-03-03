using System;

namespace Rocket.Libraries.TaskRunner.ServiceDetails
{
    public class ServiceDetail
    {
        public bool Disabled { get; set; }

        public DateTime Since { get; set; }

        public string Message { get; set; }
    }
}