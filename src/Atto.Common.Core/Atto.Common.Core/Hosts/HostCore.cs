using Atto.Common.Core.Extensions;
using Atto.Common.Core.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NLog.Web;
using Steeltoe.Extensions.Configuration.ConfigServer;
using System.IO;

namespace Atto.Common.Core.Program
{
    public class HostCore
    {
        private static string[] args;
        private static IWebHostBuilder host;
        private static string contentRoot;
        public static IWebHostBuilder CreateWebHostBuilder(string[] args, string contentRoot = default)
        {
            HostCore.args = args;
            HostCore.host = new WebHostBuilder();
            HostCore.contentRoot = contentRoot ?? Directory.GetCurrentDirectory();

            Directory.SetCurrentDirectory(HostCore.contentRoot);

            host.ConfigureServices(ConfigureServices);
            host.UseContentRoot(HostCore.contentRoot);
            host.UseKestrel();
            host.ConfigureAppConfiguration(ConfigureDelegate);
            host.AddConfigServer();
            host.ConfigureLogging(ConfigureLogging);
            host.UseNLog();
            return host;
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.Replace(ServiceDescriptor.Singleton<IServiceProviderFactory<IServiceCollection>>(new ServiceProviderFactory()));
            services.AddAutoConfigure();
        }

        private static void ConfigureLogging(WebHostBuilderContext webHostBuilderContext, ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.SetMinimumLevel(LogLevel.Trace);
            loggingBuilder.AddConsole();
            loggingBuilder.AddDebug();

            var logConfigFileName = "nlog.config";
            var logPath = Path.Combine(contentRoot, logConfigFileName);

            if (File.Exists(logPath))
                webHostBuilderContext.HostingEnvironment.ConfigureNLog("nlog.config");
        }

        private static void ConfigureDelegate(WebHostBuilderContext webHostBuilderContext, IConfigurationBuilder builder)
        {
            var env = webHostBuilderContext.HostingEnvironment;

            builder.SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFile($"secrets.json", optional: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args);
        }
    }
}