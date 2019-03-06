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
            _configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
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
            var args = new Type[] {  };
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
        public async Task ExecuteAsync_Performance_x_OriginalCommand()
        {
            var mockService = new MockService();
            var provider = new HystrixCommandProvider(_loggerFactory, _configuration);
            var options = MockService.GetCommandOptions("NormalCommand", "NormalMethod");

            var methodInfo = typeof(MockService).GetMethod(nameof(MockService.MethodWithoutParametersTestAsync), new Type[] { });
            var dic = new Dictionary<int, Tuple<TimeSpan, TimeSpan>>();

            var sequence = 1000;
            for (int i = 0; i < sequence; i++)
            {
                var stopwatch = default(Stopwatch);

                stopwatch = Stopwatch.StartNew();
                stopwatch.Start();
                var normalcommand = new NormalHystrixCommand(options, mockService, _loggerFactory).ExecuteAsync();
                var normalResult = await normalcommand;
                stopwatch.Stop();
                var ElapsedNormal = stopwatch.Elapsed;


                stopwatch = Stopwatch.StartNew();
                stopwatch.Start();
                dynamic customcommand = provider.ExecuteAsync(methodInfo, mockService);
                var customResult = await await customcommand;
                stopwatch.Stop();
                var ElapsedCustom = stopwatch.Elapsed;
                if (i == 0) continue;
                dic.Add(i, Tuple.Create(ElapsedNormal, ElapsedCustom));
            }

            ///Desvio de 13% do comando original
            ///10% por conta do Delegate Call em si e 3% do method
            var performance = dic.Values.Select(x=>new {obj = x, calc = (((x.Item2 * 100) / x.Item1) / 1000) });
            var count = performance.Where(x => x.calc <= 0.13).Count();
            count.Should().BeGreaterOrEqualTo(sequence / 2);


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