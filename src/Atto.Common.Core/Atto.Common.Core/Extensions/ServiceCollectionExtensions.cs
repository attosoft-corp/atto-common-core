using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pivotal.Discovery.Client;
using Steeltoe.CircuitBreaker.Hystrix;
using Steeltoe.Common.Http.Discovery;

namespace Atto.Common.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAttoSoft(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();

            services.AddDiscoveryClient(configuration);

            services.AddTransient<DiscoveryHttpMessageHandler>();

            services.AddMvc();

            services.AddHystrixMetricsStream(configuration);

            services.AddDefaultSwagger(configuration);

            services.AddAutoConfigure();

            return services;
        }
    }
}