using System;
using System.Collections.Generic;
using System.Text;

namespace Ruanmou.ServiceDiscovery
{
    public static class  ServiceProviderExtension
    {
        public static IServiceBuilder CreateServiceBuilder(this IServiceProvider serviceProvider, Action<IServiceBuilder> config)
        {

            ServiceBuilder builder = new ServiceBuilder(serviceProvider);
            config(builder);
            return builder;
        } 
    }
}
