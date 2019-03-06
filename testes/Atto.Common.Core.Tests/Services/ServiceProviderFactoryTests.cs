using Atto.Common.Core.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Atto.Common.Core.Tests.Services
{
    public class ServiceProviderFactoryTests
    {
        private readonly IServiceCollection _services;

        public ServiceProviderFactoryTests()
        {
            _services = new ServiceCollection();
        }

        private ServiceProviderFactory CreateFactory()
        {
            return new ServiceProviderFactory();
        }

        [Fact]
        public void CreateBuilder_TestReturnObject_ShouldReturnServiceCollection()
        {
            // Arrange
            var serviceProviderFactory = CreateFactory();

            // Act
            var result = serviceProviderFactory.CreateBuilder(_services);

            // Assert
            result.Should().BeOfType<ServiceCollection>();
        }

        [Fact]
        public void CreateServiceProvider_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var serviceProviderFactory = CreateFactory();

            // Act
            var result = serviceProviderFactory.CreateServiceProvider(_services);

            // Assert

            result.GetType().Name.Should().Be("InterceptableServiceProvider");
        }
    }
}