using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Ruanmou.ServiceDiscovery.LoadBalancer;

namespace Ruanmou.ServiceDiscovery
{
    public interface IServiceProvider
    {
        Task<IList<string>> GetServicesAsync(string serviceName);
    }
}
