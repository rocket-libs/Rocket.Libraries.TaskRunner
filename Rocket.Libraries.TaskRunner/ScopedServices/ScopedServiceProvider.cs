using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Rocket.Libraries.TaskRunner.ScopedServices
{
    public class ScopedServiceProvider : IScopedServiceProvider
    {
        public Dictionary<Type, object> scopedServices = new Dictionary<Type, object>();

        public IServiceScope Scope { get; set; }

        public TService GetService<TService>()
        {
            if (!scopedServices.ContainsKey(typeof(TService)))
            {
                scopedServices.Add(typeof(TService), Scope.ServiceProvider.GetService<TService>());
                return GetService<TService>();
            }
            else
            {
                return (TService)scopedServices[typeof(TService)];
            }
        }
    }
}