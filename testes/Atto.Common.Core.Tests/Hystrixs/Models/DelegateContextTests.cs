using Atto.Common.Core.Hystrixs.Models;
using Atto.Common.Core.Tests.Helpers;
using FluentAssertions;
using System;
using System.Reflection;
using Xunit;

namespace Atto.Common.Core.Tests.Hystrixs.Models
{
    public class DelegateContextTests
    {
        [Fact]
        public void CreateDelegate_StateUnderTest_ExpectedBehavior()
        {
            var mockService = new MockService();
            var methodInfo = typeof(MockService).GetMethod(nameof(mockService.MethodRetunsObject), new Type[] { });
            var delegateContext = new DelegateContext(methodInfo);

            var @delegate = delegateContext.CreateDelegate(mockService);

            @delegate.GetMethodInfo().Should().BeSameAs(methodInfo);
            @delegate.Target.Should().BeSameAs(mockService);
        }

        [Fact]
        public void CreateDelegate_StateUnderTest_ExpectedBehavior1()
        {
            var methodInfo = typeof(MockService).GetMethod(nameof(MockService.MethodObjectStatic), new Type[] { });
            var delegateContext = new DelegateContext(methodInfo);

            var @delegate = delegateContext.CreateDelegate();

            @delegate.GetMethodInfo().Should().BeSameAs(methodInfo);
            @delegate.Target.Should().BeNull();
        }
    }
}