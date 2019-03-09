using Atto.Common.Core.Hystrixs;
using Atto.Common.Core.Hystrixs.Interface;
using Microsoft.Extensions.DependencyInjection;

using System;

namespace Atto.Common.Core.Services
{
    public class ServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
    {
        private readonly ServiceProviderOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceProviderFactory"/> class
        /// with default options.
        /// </summary>
        /// <seealso cref="ServiceProviderOptions.Default"/>
        public ServiceProviderFactory() : this(new ServiceProviderOptions()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceProviderFactory"/> class
        /// with the specified <paramref name="options"/>.
        /// </summary>
        /// <param name="options">The options to use for this instance.</param>
        public ServiceProviderFactory(ServiceProviderOptions options)
        {
            _options = options;
        }

        public IServiceCollection CreateBuilder(IServiceCollection services)
        {
            return services;
        }

        public IServiceProvider CreateServiceProvider(IServiceCollection services)
        {
            services.AddSingleton<IHystrixCommandProvider, HystrixCommandProvider>();
            return services.BuildInterceptableServiceProvider(_options.ValidateScopes);
        }
    }
}