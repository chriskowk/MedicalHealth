using System;
using Ruanmou.ServiceDiscovery;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            // 负载均衡策略 算法
            // 几种？
            // 随机，随机是不是会不靠谱，足够大。
            // 轮询，轮询算法是可以保证所有节点都能被访问到的。
            // 加权轮询，3个节点，3,2,1
            // 动态轮询，权重值是动态的，服务监控，各个服务的访问次数
            // 最小活跃连接，
            // 一致性Hash，同一个来源（请求地址），服务链 A B C D

            // 自适应负载均衡算法
            // 最快算法，通过服务响应速度，（跨网络）
            // 观察算法，最小和最快，计算一个分数
            // 预判算法，分数记录，趋势往下走

            // 负载均衡器
            var serviceProvier = new ConsulServiceProvider(new Uri("http://127.0.0.1:8500"));
            var services = serviceProvier.GetServicesAsync("MyServiceA").Result;
            foreach (var service in services)
            {
                Console.WriteLine(service);
            }
        }
    }
}
