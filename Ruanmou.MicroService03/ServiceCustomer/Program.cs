using System;
using System.Net.Http;
using System.Threading.Tasks;
using Ruanmou.ServiceDiscovery;
using Ruanmou.ServiceDiscovery.LoadBalancer;

namespace ServiceCustomer
{
    class Program
    {
        static void Main(string[] args)
        {

            var serviceProvider = new ConsulServiceProvider(new Uri("http://127.0.0.1:8500"));
            var myServiceA = serviceProvider.CreateServiceBuilder(a =>
            {
                a.ServiceName = "MyServiceA";
                // 指定负载均衡器
                a.LoadBalancer = TypeLoadBalancer.RandomLoad;
                // 指定Uri方案
                a.UriScheme = Uri.UriSchemeHttp;
            });

            var httpClient = new HttpClient();

            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine($"-------------第{i}次请求-------------");
                try
                {
                    var uri = myServiceA.BuildAsync("/health").Result;
                    Console.WriteLine($"{DateTime.Now} - 正在调用：{uri}");
                    var content = httpClient.GetStringAsync(uri).Result;
                    Console.WriteLine($"调用结果：{content}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"调用异常：{e.GetType()}");
                }
                Task.Delay(1000).Wait();
            }
        }
    }
}
