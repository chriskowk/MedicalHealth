using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ruanmou.ServiceDiscovery.LoadBalancer
{
    public interface ILoadBalancer
    {
        string Resolve(IList<string> services);
    }
}
