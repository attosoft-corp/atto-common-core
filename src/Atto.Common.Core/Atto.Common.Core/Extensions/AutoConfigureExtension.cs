using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Atto.Common.Core.Extensions
{
    internal static class AutoConfigureExtension
    {
        public static IServiceCollection AddAutoConfigure(this IServiceCollection services)
        {
            var autoConfigConventions = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes().Where(t => t.Name.EndsWith("AutoConfig") && !t.IsInterface && !t.IsAbstract)).ToList();

            var provider = services.BuildServiceProvider();

            autoConfigConventions.ForEach(type =>
            {
                var instance = ActivatorUtilities.GetServiceOrCreateInstance(provider, type);
                var configureServicesMethodInfo = type.GetMethod(nameof(IStartup.ConfigureServices), new Type[] { typeof(IServiceCollection) });
                services = (IServiceCollection)configureServicesMethodInfo.Invoke(instance, new object[] { services });
                services.AddSingleton(instance);
            });

            return services;
        }
    }
}