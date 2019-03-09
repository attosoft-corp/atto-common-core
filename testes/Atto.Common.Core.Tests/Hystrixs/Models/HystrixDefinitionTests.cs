using Atto.Common.Core.Hystrixs.Interface;
using Atto.Common.Core.Hystrixs.Models;
using Atto.Common.Core.Tests.Helpers;
using AutoFixture;
using FluentAssertions;
using NSubstitute;
using Steeltoe.CircuitBreaker.Hystrix;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace Atto.Common.Core.Tests.Hystrixs.Models
{
    public class HystrixDefinitionTests
    {
        private readonly IFixture _fixture;
        private readonly IHystrixCommandOptions _hystrixCommandOptions;

        public HystrixDefinitionTests()
        {
            _fixture = new Fixture();
            _hystrixCommandOptions = Substitute.For<IHystrixCommandOptions>();
        }

        private IHystrixDefinition CreateHystrixDefinition(MethodInfo methodInfo) =>
            new HystrixDefinition(_hystrixCommandOptions, methodInfo, this);

        [Fact]
        public void Should_Load_HystrixTypeAsDefinedMethodInfo()
        {
            var mockServiceType = typeof(MockService);

            MethodInfo methodInfo = mockServiceType.GetMethod(nameof(MockService.MethodReturnsTask), new Type[] { });
            IHystrixDefinition definition = CreateHystrixDefinition(methodInfo);
            definition.HystrixBaseType.Should().Be(typeof(HystrixCommandAsyncBase));

            methodInfo = mockServiceType.GetMethod(nameof(MockService.MethodRetunsTaskObject), new Type[] { });
            definition = CreateHystrixDefinition(methodInfo);
            definition.HystrixBaseType.Should().Be(typeof(HystrixCommandAsyncBase<object>));

            methodInfo = mockServiceType.GetMethod(nameof(MockService.MethodWithGenericTypetTest), new Type[] { });
            definition = CreateHystrixDefinition(methodInfo);
            definition.HystrixBaseType.Should().Be(typeof(HystrixCommandBase<IEnumerable<object>>));

            methodInfo = mockServiceType.GetMethod(nameof(MockService.MethodRetunsVoid), new Type[] { });
            definition = CreateHystrixDefinition(methodInfo);
            definition.HystrixBaseType.Should().Be(typeof(HystrixCommandBase));

            methodInfo = mockServiceType.GetMethod(nameof(MockService.MethodRetunsObject), new Type[] { });
            definition = CreateHystrixDefinition(methodInfo);
            definition.HystrixBaseType.Should().Be(typeof(HystrixCommandBase<object>));

            methodInfo = mockServiceType.GetMethod(nameof(MockService.MethodReturnsTaskStatic), new Type[] { });
            definition = CreateHystrixDefinition(methodInfo);
            definition.HystrixBaseType.Should().Be(typeof(HystrixCommandAsyncBase));

            methodInfo = mockServiceType.GetMethod(nameof(MockService.MethodRetunsTaskObjectStatic), new Type[] { });
            definition = CreateHystrixDefinition(methodInfo);
            definition.HystrixBaseType.Should().Be(typeof(HystrixCommandAsyncBase<object>));

            methodInfo = mockServiceType.GetMethod(nameof(MockService.MethodVoidStatic), new Type[] { });
            definition = CreateHystrixDefinition(methodInfo);
            definition.HystrixBaseType.Should().Be(typeof(HystrixCommandBase));

            methodInfo = mockServiceType.GetMethod(nameof(MockService.MethodObjectStatic), new Type[] { });
            definition = CreateHystrixDefinition(methodInfo);
            definition.HystrixBaseType.Should().Be(typeof(HystrixCommandBase<object>));
        }

        [Fact]
        public void CheckRequiredProperties()
        {
            var action = default(Action);
            var methodInfo = typeof(MockService).GetMethod(nameof(MockService.MethodReturnsTask), new Type[] { });

            action = () => new HystrixDefinition(null, methodInfo, this);
            action.Should().ThrowExactly<ArgumentNullException>();

            action = () => new HystrixDefinition(_hystrixCommandOptions, null, this);
            action.Should().ThrowExactly<ArgumentNullException>();

            action = () => new HystrixDefinition(_hystrixCommandOptions, methodInfo, null);
            action.Should().ThrowExactly<ArgumentNullException>();
        }
    }
}