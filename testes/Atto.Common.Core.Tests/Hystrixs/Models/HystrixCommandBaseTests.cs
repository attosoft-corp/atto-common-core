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
using Xunit;

namespace Atto.Common.Core.Tests.Hystrixs.Models
{
    public class HystrixCommandBaseTests
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IFixture _fixture;

        public HystrixCommandBaseTests()
        {
            _loggerFactory = Substitute.For<ILoggerFactory>();
            _fixture = new Fixture();
        }

        [Fact]
        public void Execute_ShouldExecute_FallbackWhenTimeoutOnPrimary()
        {
            // Arrange
            var executationResult = string.Empty;

            var mockService = new MockService();

            var methodInfo = default(MethodInfo);

            var primaryArgs = new Type[] { typeof(int) };

            methodInfo = typeof(MockService).GetMethod(nameof(MockService.MethodWithTimeoutTest), primaryArgs);

            var primaryFuncArgs = primaryArgs.Concat(new Type[] { methodInfo.ReturnType }).ToArray();

            var primaryDelegate = methodInfo.CreateDelegate(Expression.GetDelegateType(primaryFuncArgs), mockService);

            var fallbackArgs = new Type[] { typeof(HystrixFallback), typeof(int) };

            methodInfo = typeof(MockService).GetMethod(nameof(MockService.MethodWithTimeoutTest), fallbackArgs);

            var fallbackFuncArgs = fallbackArgs.Concat(new Type[] { methodInfo.ReturnType }).ToArray();

            var fallbackDelegate = methodInfo.CreateDelegate(Expression.GetFuncType(fallbackFuncArgs), mockService);

            var options = MockService.GetCommandOptions(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            //Act
            var command = new HystrixCommandBase<IEnumerable<Type>>(options, primaryDelegate, fallbackDelegate, new object[] { 100000 }, _loggerFactory);
            var result = command.Execute();

            // Assert
            result.Should().BeEquivalentTo(fallbackArgs);
        }

        [Fact]
        public void ExecuteAsync_ShouldExecute_PrimaryWhenNoTimeoutOnIt()
        {
            // Arrange
            var executationResult = string.Empty;

            var mockService = new MockService();

            var methodInfo = default(MethodInfo);

            var primaryArgs = new Type[] { typeof(int) };

            methodInfo = typeof(MockService).GetMethod(nameof(MockService.MethodWithTimeoutTest), primaryArgs);

            var primaryFuncArgs = primaryArgs.Concat(new Type[] { methodInfo.ReturnType }).ToArray();

            var primaryDelegate = methodInfo.CreateDelegate(Expression.GetDelegateType(primaryFuncArgs), mockService);

            var fallbackArgs = new Type[] { typeof(HystrixFallback), typeof(int) };

            methodInfo = typeof(MockService).GetMethod(nameof(MockService.MethodWithTimeoutTest), fallbackArgs);

            var fallbackFuncArgs = fallbackArgs.Concat(new Type[] { methodInfo.ReturnType }).ToArray();

            var fallbackDelegate = methodInfo.CreateDelegate(Expression.GetFuncType(fallbackFuncArgs), mockService);

            var options = MockService.GetCommandOptions(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            //Act
            var command = new HystrixCommandBase<IEnumerable<Type>>(options, primaryDelegate, fallbackDelegate, new object[] { 10 }, _loggerFactory);
            var result = command.Execute();

            // Assert
            result.Should().BeEquivalentTo(primaryArgs);
        }

        [Fact]
        public void ExecuteAsync_ShouldExecute_MethodWithOutParametersRetuningVoid()
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
            var command = new HystrixCommandBase(options, primaryDelegate, fallbackDelegate, new object[] { }, _loggerFactory);

            command.Execute();
        }
    }
}