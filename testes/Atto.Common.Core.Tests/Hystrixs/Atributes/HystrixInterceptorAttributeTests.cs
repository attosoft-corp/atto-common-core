using Atto.Common.Core.Attributes;
using Atto.Common.Core.Hystrixs;
using Atto.Common.Core.Hystrixs.Interface;
using Atto.Common.Core.Tests.Helpers;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Atto.Common.Core.Tests.Hystrixs.Atributes
{
    public class HystrixInterceptorAttributeTests
    {
        public HystrixInterceptorAttributeTests()
        {
        }

        private HystrixInterceptorAttribute CreateHystrixInterceptorAttribute()
        {
            return new HystrixInterceptorAttribute();
        }

        [Fact]
        public void Use_StateUnderTest_ExpectedBehavior()
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
            var services = new ServiceCollection();
            services.AddTransient<IMockService, MockService>();
            services.AddLogging();
            services.AddOptions();

            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton<IHystrixCommandProvider, HystrixCommandProvider>();
            var provider = services.BuildInterceptableServiceProvider();

            var mockService = provider.GetService<IMockService>();
            var result = mockService.MethodWithInterceptAttribute();

            result.Should().NotBeNull();
        }
    }
}