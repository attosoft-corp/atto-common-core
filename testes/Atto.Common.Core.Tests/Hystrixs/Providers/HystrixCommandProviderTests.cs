using Atto.Common.Core.Hystrixs;
using Atto.Common.Core.Hystrixs.Models;
using Atto.Common.Core.Tests.Helpers;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Steeltoe.CircuitBreaker.Hystrix;
using Steeltoe.CircuitBreaker.Hystrix.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Xunit;

namespace Atto.Common.Core.Tests.Hystrixs.Providers
{
    public class HystrixCommandProviderTests
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IConfiguration _configuration;
        private readonly IFixture _fixture;
        public static bool contextVariable;

        public HystrixCommandProviderTests()
        {
            _loggerFactory = Substitute.For<ILoggerFactory>();
            var param = new Dictionary<string, string>
            {
                { "hystrix:command:default:execution:isolation:thread:timeoutInMilliseconds", "60000" },
                { "hystrix:command:default:circuitBreaker", "true" }
            };
            _configuration = new ConfigurationBuilder().AddInMemoryCollection(param).Build();
            _fixture = new Fixture();

        }

        private HystrixCommandProvider CreateProvider()
        {
            return new HystrixCommandProvider(_loggerFactory, _configuration);
        }

        [Fact]
        public async Task ExecuteAsync_Testing_Method_WithParametersAsync()
        {
            // Arrange
            var provider = CreateProvider();
            var args = new Type[] { typeof(int), typeof(string) };
            var methodInfo = typeof(MockService).GetMethod(nameof(MockService.MethodWithParametersTestAsync), args);
            var mockService = new MockService();

            // Act
            dynamic task = await provider.ExecuteAsync(methodInfo, mockService, new object[] { _fixture.Create<int>(), _fixture.Create<string>() });
            IEnumerable<Type> result = await task;

            // Assert
            result.Should().BeEquivalentTo(args);
        }

        [Fact]
        public async Task ExecuteAsync_Testing_Method_WithOutParametersStatic()
        {

            // Arrange
            var provider = CreateProvider();
            var args = new Type[] { };
            var methodInfo = typeof(MockService).GetMethod(nameof(MockService.MethodObjectStatic), args);
            var mockService = new MockService();

            // Act
            var returnObject = await provider.ExecuteAsync(methodInfo, mockService, new object[] { });

            // Assert
            returnObject.Should().NotBeNull();
        }

        [Fact]
        public async Task ExecuteAsync_Testing_Method_WithoutParametersAsync()
        {
            // Arrange
            var provider = CreateProvider();
            var args = new Type[] { };
            var methodInfo = typeof(MockService).GetMethod(nameof(MockService.MethodWithoutParametersTestAsync), args);
            var mockService = new MockService();

            // Act
            dynamic task = await provider.ExecuteAsync(methodInfo, mockService, new object[] { });
            IEnumerable<Type> result = await task;

            // Assert
            result.Should().BeEquivalentTo(args);
        }

        [Fact]
        public async Task ExecuteAsync_Testing_Method_MethodWithExceptionTest_ExecuteFallbackAsync()
        {
            // Arrange
            var provider = CreateProvider();
            var args = new Type[] { typeof(long), typeof(bool) };
            var methodInfo = typeof(MockService).GetMethod(nameof(MockService.MethodWithExceptionTestAsync), args);
            var mockService = new MockService();

            // Act
            dynamic task = await provider.ExecuteAsync(methodInfo, mockService, new object[] { _fixture.Create<long>(), _fixture.Create<bool>() });
            IEnumerable<Type> result = await task;

            // Assert
            var fallbackArgs = new Type[] { typeof(HystrixFallback) };

            var resultArgs = fallbackArgs.Concat(args);
            result.Should().BeEquivalentTo(resultArgs);
        }

        [Fact]
        public async Task ExecuteAsync_Testing_Method_WithoutTaskReturnsAsync()
        {
            // Arrange
            var provider = CreateProvider();
            var args = new Type[] { };
            var methodInfo = typeof(MockService).GetMethod(nameof(MockService.MethodReturnsTask), args);
            var mockService = Substitute.For<IMockService>();
            var ExecutionResult = default(string);
            mockService.When(x => x.MethodReturnsTask()).Do(x => ExecutionResult = "PrimaryMethodCalled");

            // Act
            dynamic task = await provider.ExecuteAsync(methodInfo, mockService, new object[] { });
            await task;

            // Assert
            Task result = task;
            result.Should().BeOfType<Task<Unit>>();
            ExecutionResult.Should().Be("PrimaryMethodCalled");
        }

        [Fact]
        public async Task ExecuteAsync_Testing_Method_WithVoidReturnsAsync()
        {
            // Arrange
            var provider = CreateProvider();
            var args = new Type[] { };
            var methodInfo = typeof(MockService).GetMethod(nameof(MockService.MethodRetunsVoid), args);
            var mockService = Substitute.For<IMockService>();
            var ExecutionResult = default(string);
            mockService.When(x => x.MethodRetunsVoid()).Do(x => ExecutionResult = "PrimaryMethodCalled");

            // Act
            var result = await provider.ExecuteAsync(methodInfo, mockService, new object[] { });

            // Assert
            result.Should().BeNull();
            ExecutionResult.Should().Be("PrimaryMethodCalled");
        }

        [Fact]
        public async Task ExecuteAsync_MethodWithExceptionWithoutFallback_ShouldThrowException()
        {
            // Arrange
            var provider = CreateProvider();
            var args = new Type[] { };
            var methodInfo = typeof(MockService).GetMethod(nameof(MockService.MethodWithExceptionWithoutFallbackTestAsync), args);
            var mockService = new MockService();

            // Act
            Action action = () =>
            {
                dynamic result = provider.ExecuteAsync(methodInfo, mockService).GetAwaiter().GetResult();
                result.GetAwaiter().GetResult();
            };

            // Assert
            action.Should().ThrowExactly<HystrixRuntimeException>();

            await Task.Yield();
        }

        [Fact]
        public void Test_performance()
        {
            var mockService = new MockService();
            var provider = new HystrixCommandProvider(_loggerFactory, _configuration);
            var options = MockService.GetCommandOptions("NormalCommand", "NormalMethod");

            var methodInfo = typeof(MockService).GetMethod(nameof(MockService.MethodWithoutParametersTestAsync), new Type[] { });


            var sequence = 10000;


            var customCommandTasks = Enumerable.Range(0, sequence).Select(async i =>
            {
                dynamic customcommand = provider.ExecuteAsync(methodInfo, mockService);
                var customResult = await await customcommand;
                return customResult;
            });

            var normalCommandTasks = Enumerable.Range(0, sequence).Select(i =>
            {
                var command = new NormalHystrixCommand(options, mockService, _loggerFactory);
                return command.ExecuteAsync();
            });



            var stopwatch = Stopwatch.StartNew();
            var customresult = Task.WhenAll(customCommandTasks.ToArray()).Result;
            var customTimeElapse = stopwatch.Elapsed;

            stopwatch.Reset();

            stopwatch.Start();
            var normalResult = Task.WhenAll(normalCommandTasks.ToArray()).Result;
            var normalTimeElapse = stopwatch.Elapsed;

            var dif = customTimeElapse.TotalSeconds - normalTimeElapse.TotalSeconds;

            using (var writer = new StreamWriter("c:/temp/teste.log", true))
            {
                writer.WriteLine("Result of time: " + dif.ToString());
            }
            dif.Should().BeInRange(-2.30, 2.30);
        }
    }

    public class NormalHystrixCommand : HystrixCommand<IEnumerable<Type>>
    {
        private readonly IMockService _mockService;

        public NormalHystrixCommand(IHystrixCommandOptions options, IMockService mockService, ILoggerFactory loggerFactory) : base(options, logger: loggerFactory.CreateLogger<NormalHystrixCommand>())
        {
            _mockService = mockService;
        }

        protected override Task<IEnumerable<Type>> RunAsync()
        {
            return _mockService.MethodWithoutParametersTestAsync();
        }

        protected override Task<IEnumerable<Type>> RunFallbackAsync()
        {
            return _mockService.MethodWithoutParametersTestAsync(new HystrixFallback());
        }
    }
}