using Atto.Common.Core.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NLog.Web;
using Steeltoe.Extensions.Configuration.ConfigServer;

namespace Microsoft.AspNetCore
{
    public static class HostCore
    {
        public static IWebHostBuilder UseAttoSoft(this IWebHostBuilder host)
        {
            host.ConfigureServices(ConfigureServices);
            host.ConfigureAppConfiguration(ConfigureDelegate);
            host.AddConfigServer();
            host.UseNLog();

            return host;
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.Replace(ServiceDescriptor.Singleton<IServiceProviderFactory<IServiceCollection>>(new ServiceProviderFactory()));
        }

        private static void ConfigureDelegate(WebHostBuilderContext webHostBuilderContext, IConfigurationBuilder builder)
        {
            var env = webHostBuilderContext.HostingEnvironment;

            env.ConfigureNLog("nlog.config");

            builder.SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFile($"secrets.json", optional: true)
                .AddEnvironmentVariables();
        }
    }
}