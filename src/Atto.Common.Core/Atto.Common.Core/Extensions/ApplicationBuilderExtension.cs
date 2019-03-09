using Microsoft.AspNetCore.Builder;
using Pivotal.Discovery.Client;
using Steeltoe.CircuitBreaker.Hystrix;

namespace Atto.Common.Core.Extensions
{
    public static class ApplicationBuilderExtension
    {
        public static void UseAttoSoft(this IApplicationBuilder app)
        {
            app.UseHystrixRequestContext();

            app.UseDiscoveryClient();

            app.UseMvc();

            app.UseHystrixMetricsStream();

            app.UseDefaultSwagger();

            app.UseAutoConfigure();
        }
    }
}