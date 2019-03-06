using Atto.Common.Core.Hystrixs.Interface;
using Atto.Common.Core.Interceptors;
using Atto.Common.Core.Tests.Helpers;
using Dora.DynamicProxy;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Atto.Common.Core.Tests.Interceptors
{
    public class HystrixInterceptorTests
    {
        private readonly ILoggerFactory _loggerFactory;
        private IHystrixCommandProvider _hystrixCommandProvider;

        public HystrixInterceptorTests()
        {
            _loggerFactory = Substitute.For<ILoggerFactory>();
            _hystrixCommandProvider = Substitute.For<IHystrixCommandProvider>();
        }

        private HystrixInterceptor CreateHystrixInterceptor()
        {
            return new HystrixInterceptor(_loggerFactory, _hystrixCommandProvider);
        }

        [Fact]
        public async Task InvokeAsync_ShouldExecute_LoadRetunValueAsString()
        {
            // Arrange
            var hystrixInterceptor = CreateHystrixInterceptor();
            var methodInfo = typeof(MockService).GetMethod(nameof(MockService.MethodRetunsObject), new Type[] { });
            var mockservice = new MockService();
            var context = new DefaultInvocationContext(methodInfo, mockservice, mockservice, new object[] { });
            _hystrixCommandProvider.ExecuteAsync(methodInfo, Arg.Any<object>(), Arg.Any<object[]>()).Returns(Task.FromResult<object>("Teste"));

            // Act
            await hystrixInterceptor.InvokeAsync(context);

            // Assert
            context.ReturnValue.Should().NotBeNull();
        }
    }
}