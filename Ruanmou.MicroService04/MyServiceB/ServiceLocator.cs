using Microsoft.AspNetCore.Builder;

namespace MyServiceB
{
    public static class ServiceLocator
    {
        public static IApplicationBuilder ApplicationBuilder { get; set; }
    }
}
