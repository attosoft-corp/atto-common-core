using Atto.Common.Core.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Atto.Common.Core.Tests.Extensions
{
    public class StartupTest
    {
        public StartupTest(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAttoSoft(Configuration);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseAttoSoft();
        }
    }
}