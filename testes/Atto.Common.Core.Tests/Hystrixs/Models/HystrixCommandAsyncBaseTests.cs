using Atto.Common.Core.Hystrixs.Models;
using Atto.Common.Core.Tests.Helpers;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Atto.Common.Core.Tests.Hystrixs.Models
{
    public class HystrixCommandAsyncBaseTests
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IFixture _fixture;

        public HystrixCommandAsyncBaseTests()
        {
            _loggerFactory = Substitute.For<ILoggerFactory>();
            _fixture = new Fixture();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldExecute_FallbackWhenTimeoutOnPrimary()
        {
            // Arrange

            var mockService = new MockService();
            var primaryArgs = new Type[] { typeof(int) };

            MethodInfo methodInfo = typeof(MockService).GetMethod(nameof(MockService.MethodWithTimeoutTestAsync), primaryArgs);

            var primaryFuncArgs = primaryArgs.Concat(new Type[] { methodInfo.ReturnType }).ToArray();

            var primaryDelegate = methodInfo.CreateDelegate(Expression.GetDelegateType(primaryFuncArgs), mockService);

            var fallbackArgs = new Type[] { typeof(HystrixFallback), typeof(int) };

            methodInfo = typeof(MockService).GetMethod(nameof(MockService.MethodWithTimeoutTestAsync), fallbackArgs);

            var fallbackFuncArgs = fallbackArgs.Concat(new Type[] { methodInfo.ReturnType }).ToArray();

            var fallbackDelegate = methodInfo.CreateDelegate(Expression.GetFuncType(fallbackFuncArgs), mockService);

            var options = MockService.GetCommandOptions(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            //Act
            var command = new HystrixCommandAsyncBase<IEnumerable<Type>>(options, primaryDelegate, fallbackDelegate, new object[] { 100000 }, _loggerFactory);
            var result = await command.ExecuteAsync();

            // Assert
            result.Should().BeEquivalentTo(fallbackArgs);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldExecute_PrimaryWhenNoTimeoutOnIt()
        {
            // Arrange

            var mockService = new MockService();
            var primaryArgs = new Type[] { typeof(int) };

            MethodInfo methodInfo = typeof(MockService).GetMethod(nameof(MockService.MethodWithTimeoutTestAsync), primaryArgs);

            var primaryFuncArgs = primaryArgs.Concat(new Type[] { methodInfo.ReturnType }).ToArray();

            var primaryDelegate = methodInfo.CreateDelegate(Expression.GetDelegateType(primaryFuncArgs), mockService);

            var fallbackArgs = new Type[] { typeof(HystrixFallback), typeof(int) };

            methodInfo = typeof(MockService).GetMethod(nameof(MockService.MethodWithTimeoutTestAsync), fallbackArgs);

            var fallbackFuncArgs = fallbackArgs.Concat(new Type[] { methodInfo.ReturnType }).ToArray();

            var fallbackDelegate = methodInfo.CreateDelegate(Expression.GetFuncType(fallbackFuncArgs), mockService);

            var options = MockService.GetCommandOptions(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            //Act
            var command = new HystrixCommandAsyncBase<IEnumerable<Type>>(options, primaryDelegate, fallbackDelegate, new object[] { 10 }, _loggerFactory);
            var result = await command.ExecuteAsync();

            // Assert
            result.Should().BeEquivalentTo(primaryArgs);
        }

        [Fact]
        public void ExecuteAsync_ShouldExecute_FallbackWhenTimeoutOnPrimaryVoid()
        {
            // Arrange

            var mockService = new MockService();
            var primaryArgs = new Type[] { };

            MethodInfo methodInfo = typeof(MockService).GetMethod(nameof(MockService.MethodRetunsVoid), primaryArgs);

            var primaryFuncArgs = primaryArgs.Concat(new Type[] { methodInfo.ReturnType }).ToArray();

            var primaryDelegate = methodInfo.CreateDelegate(Expression.GetDelegateType(primaryFuncArgs), mockService);

            var fallbackArgs = new Type[] { typeof(HystrixFallback) };

            methodInfo = typeof(MockService).GetMethod(nameof(MockService.MethodRetunsVoid), fallbackArgs);

            var fallbackDelegate = methodInfo.CreateDelegate(Expression.GetActionType(fallbackArgs), mockService);

            var options = MockService.GetCommandOptions(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            //Act
            var command = new HystrixCommandAsyncBase(options, primaryDelegate, fallbackDelegate, new object[] { }, _loggerFactory);
            command.Execute();
        }

        [Fact]
        public void ExecuteAsync_ShouldExecute_MethodWithParameters()
        {
            // Arrange

            var mockService = new MockService();
            var primaryArgs = new Type[] { typeof(int), typeof(string) };

            MethodInfo methodInfo = typeof(MockService).GetMethod(nameof(MockService.MethodWithParametersTestAsync), primaryArgs);

            var primaryFuncArgs = primaryArgs.Concat(new Type[] { methodInfo.ReturnType }).ToArray();

            var primaryDelegate = methodInfo.CreateDelegate(Expression.GetDelegateType(primaryFuncArgs), mockService);

            var fallbackArgs = new Type[] { typeof(HystrixFallback) };

            methodInfo = typeof(MockService).GetMethod(nameof(MockService.MethodRetunsVoid), fallbackArgs);

            var fallbackDelegate = methodInfo.CreateDelegate(Expression.GetActionType(fallbackArgs), mockService);

            var options = MockService.GetCommandOptions(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            //Act
            var command = new HystrixCommandAsyncBase(options, primaryDelegate, fallbackDelegate, new object[] { }, _loggerFactory);
            command.Execute();
        }

        [Fact]
        public void ExecuteAsync_ShouldExecute_MethodWithOutParameters()
        {
            // Arrange

            var mockService = new MockService();
            var primaryArgs = new Type[] {  };

            MethodInfo methodInfo = typeof(MockService).GetMethod(nameof(MockService.MethodWithoutParametersTestAsync), primaryArgs);

            var primaryFuncArgs = primaryArgs.Concat(new Type[] { methodInfo.ReturnType }).ToArray();

            var primaryDelegate = methodInfo.CreateDelegate(Expression.GetDelegateType(primaryFuncArgs), mockService);

            var fallbackArgs = new Type[] { typeof(HystrixFallback) };

            methodInfo = typeof(MockService).GetMethod(nameof(MockService.MethodRetunsVoid), fallbackArgs);

            var fallbackDelegate = methodInfo.CreateDelegate(Expression.GetActionType(fallbackArgs), mockService);

            var options = MockService.GetCommandOptions(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            //Act
            var command = new HystrixCommandAsyncBase(options, primaryDelegate, fallbackDelegate, new object[] { }, _loggerFactory);

            command.Execute();
        }

        [Fact]
        public void ExecuteAsync_ShouldExecute_MethodWithOutParametersRetuningTaskObject()
        {
            // Arrange

            var mockService = new MockService();
            var primaryArgs = new Type[] { };

            MethodInfo methodInfo = typeof(MockService).GetMethod(nameof(MockService.MethodRetunsTaskObject), primaryArgs);

            var primaryFuncArgs = primaryArgs.Concat(new Type[] { methodInfo.ReturnType }).ToArray();

            var primaryDelegate = methodInfo.CreateDelegate(Expression.GetDelegateType(primaryFuncArgs), mockService);

            var fallbackArgs = new Type[] { typeof(HystrixFallback) };

            methodInfo = typeof(MockService).GetMethod(nameof(MockService.MethodRetunsVoid), fallbackArgs);

            var fallbackDelegate = methodInfo.CreateDelegate(Expression.GetActionType(fallbackArgs), mockService);

            var options = MockService.GetCommandOptions(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            //Act
            var command = new HystrixCommandAsyncBase(options, primaryDelegate, fallbackDelegate, new object[] { }, _loggerFactory);

            command.Execute();
        }

        [Fact]
        public async Task ExecuteAsync_ShouldExecute_MethodWithOutParametersRetuningTask()
        {
            // Arrange

            var mockService = new MockService();
            var primaryArgs = new Type[] { };

            MethodInfo methodInfo = typeof(MockService).GetMethod(nameof(MockService.MethodReturnsTask), primaryArgs);

            var primaryFuncArgs = primaryArgs.Concat(new Type[] { methodInfo.ReturnType }).ToArray();

            var primaryDelegate = methodInfo.CreateDelegate(Expression.GetDelegateType(primaryFuncArgs), mockService);

            var fallbackArgs = new Type[] { typeof(HystrixFallback) };

            methodInfo = typeof(MockService).GetMethod(nameof(MockService.MethodRetunsVoid), fallbackArgs);

            var fallbackDelegate = methodInfo.CreateDelegate(Expression.GetActionType(fallbackArgs), mockService);

            var options = MockService.GetCommandOptions(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            //Act
            var command = new HystrixCommandAsyncBase(options, primaryDelegate, fallbackDelegate, new object[] { }, _loggerFactory);

            await command.ExecuteAsync();
        }
    }
}