using System.Linq;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace MyServiceB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {

        public ActionResult Get()
        {
            // 获取当前服务地址和端口
            var features = ServiceLocator.ApplicationBuilder.Properties["server.Features"] as FeatureCollection;
            var address = features.Get<IServerAddressesFeature>().Addresses.First();
            return Ok($"Order From MyServiceB[{address}]");
        }
    }
} 