using Atto.Common.Core.Hystrixs;
using Atto.Common.Core.Hystrixs.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pivotal.Discovery.Client;
using Steeltoe.CircuitBreaker.Hystrix;
using Steeltoe.Common.Http.Discovery;

namespace Atto.Common.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDefaultCollections(this IServiceCollection services, IConfiguration configuration)
        {
            //Add IOptions to the pipeline
            services.AddOptions();

            //Add Eureka for Discovery services
            services.AddDiscoveryClient(configuration);

            // Add Steeltoe handler to container
            services.AddTransient<DiscoveryHttpMessageHandler>();

            // Add Mvc
            services.AddMvc();

            // Add Hystrix metrics stream to enable monitoring
            services.AddHystrixMetricsStream(configuration);

            //Add commands to the pipeline
            services.AddSingleton<IHystrixCommandProvider, HystrixCommandProvider>();

            services.AddDefaultSwagger(configuration);

            services.AddAutoConfigure();

            return services;
        }
    }
}