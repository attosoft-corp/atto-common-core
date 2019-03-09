using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Atto.Common.Core.Tests.Helpers
{
    public class TestAutoConfig
    {
        public IServiceCollection ConfigureServices(IServiceCollection services)
        {
            return services;
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseStaticFiles();
        }
    }
}