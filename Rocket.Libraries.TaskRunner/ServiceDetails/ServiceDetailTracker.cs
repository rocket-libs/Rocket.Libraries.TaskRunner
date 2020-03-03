using System;

namespace Rocket.Libraries.TaskRunner.ServiceDetails
{
    public static class ServiceDetailTracker
    {
        private static ServiceDetail serviceDetail;

        public static ServiceDetail ServiceDetail
        {
            get
            {
                if (serviceDetail == null)
                {
                    serviceDetail = new ServiceDetail();
                }
                return serviceDetail;
            }
        }

        internal static void WriteStatus(bool disabled, DateTime since, string message)
        {
            ServiceDetail.Disabled = disabled;
            ServiceDetail.Since = since;
            ServiceDetail.Message = message;
        }
    }
}