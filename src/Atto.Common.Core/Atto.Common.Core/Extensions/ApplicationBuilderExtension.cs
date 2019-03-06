using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Pivotal.Discovery.Client;
using Steeltoe.CircuitBreaker.Hystrix;

namespace Atto.Common.Core.Extensions
{
    public static class ApplicationBuilderExtension
    {
        public static void UseDefaultApplications(this IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseHystrixRequestContext();

            app.UseHttpsRedirection();

            app.UseDiscoveryClient();

            app.UseMvc();

            app.UseHystrixMetricsStream();

            app.UseDefaultSwagger();

        }
    }
}