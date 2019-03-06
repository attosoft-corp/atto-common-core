using Atto.Common.Core.Attributes;
using Atto.Common.Core.Hystrixs.Models;
using Microsoft.Extensions.Configuration;
using Steeltoe.CircuitBreaker.Hystrix;
using Steeltoe.CircuitBreaker.Hystrix.Strategy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Atto.Common.Core.Tests.Helpers
{
    internal class MockService : IMockService
    {
        public static IHystrixCommandOptions GetCommandOptions(string serviceName, string methodName)
        {
            var configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
            var strategy = HystrixPlugins.OptionsStrategy;
            var dynOpts = strategy.GetDynamicOptions(configuration);

            var commandKey = HystrixCommandKeyDefault.AsKey($"{serviceName}.{methodName}");
            var groupKey = HystrixCommandGroupKeyDefault.AsKey($"{serviceName}Group");

            IHystrixCommandOptions opts = new HystrixCommandOptions(groupKey, commandKey, null, dynOpts)
            {
                ThreadPoolKey = HystrixThreadPoolKeyDefault.AsKey($"{serviceName}Group")
            };
            opts.ThreadPoolOptions = new HystrixThreadPoolOptions(opts.ThreadPoolKey, null, dynOpts);

            return opts;
        }

        public static object MethodObjectStatic() => new object();

        public static Task<object> MethodRetunsTaskObjectStatic() => Task.FromResult(new object());

        public static Task MethodReturnsTaskStatic() => Task.CompletedTask;

        public static void MethodVoidStatic() { }

        public object MethodRetunsObject() => new object();

        public Task<object> MethodRetunsTaskObject() => Task.FromResult(new object());

        public void MethodRetunsVoid() => throw new Exception();

        public void MethodRetunsVoid(HystrixFallback hystrixFallback) { }

        public Task MethodReturnsTask() => Task.CompletedTask;
        public Task MethodWithCancelationTokenTest(CancellationToken token) => Task.CompletedTask;

        public Task MethodWithCancelationTokenTest(HystrixFallback hystrixFallback, CancellationToken token) => Task.CompletedTask;

        public Task<IEnumerable<Type>> MethodWithExceptionTestAsync(long a, bool b) => Task.FromException<IEnumerable<Type>>(new Exception("this is a fail on primary method"));

        public Task<IEnumerable<Type>> MethodWithExceptionTestAsync(HystrixFallback hystrixFallback, long a, bool b) => Task.FromResult(MethodBase.GetCurrentMethod().GetParameters().Select(x => x.ParameterType));

        public Task<IEnumerable<Type>> MethodWithExceptionWithoutFallbackTestAsync() => Task.FromException<IEnumerable<Type>>(new Exception("this will be throw because this method doesnt have fallback"));

        public Task<IEnumerable<Type>> MethodWithoutParametersTestAsync() => Task.FromResult(MethodBase.GetCurrentMethod().GetParameters().Select(x => x.ParameterType));

        public Task<IEnumerable<Type>> MethodWithoutParametersTestAsync(HystrixFallback hystrixFallback) => Task.FromResult(MethodBase.GetCurrentMethod().GetParameters().Select(x => x.ParameterType));

        public Task<IEnumerable<Type>> MethodWithParametersTestAsync(int a, string b) => Task.FromResult(MethodBase.GetCurrentMethod().GetParameters().Select(x => x.ParameterType));

        public Task<IEnumerable<Type>> MethodWithParametersTestAsync(HystrixFallback hystrixFallback, int a, int b) => Task.FromResult(MethodBase.GetCurrentMethod().GetParameters().Select(x => x.ParameterType));
        public IEnumerable<Type> MethodWithTimeoutTest(int a)
        {
            Thread.Sleep(a);
            return MethodBase.GetCurrentMethod().GetParameters().Select(x => x.ParameterType);
        }

        public IEnumerable<Type> MethodWithTimeoutTest(HystrixFallback hystrixFallback, int a) => MethodBase.GetCurrentMethod().GetParameters().Select(x => x.ParameterType);

        public Task<IEnumerable<Type>> MethodWithTimeoutTestAsync(int a)
        {
            Thread.Sleep(a);
            return Task.FromResult(MethodBase.GetCurrentMethod().GetParameters().Select(x => x.ParameterType));
        }

        public Task<IEnumerable<Type>> MethodWithTimeoutTestAsync(HystrixFallback hystrixFallback, int a) => Task.FromResult(MethodBase.GetCurrentMethod().GetParameters().Select(x => x.ParameterType));

        public IEnumerable<object> MethodWithGenericTypetTest() => MethodBase.GetCurrentMethod().GetParameters().Select(x => x.ParameterType);
        public IEnumerable<object> MethodWithGenericTypetTest(HystrixFallback hystrixFallback) => MethodBase.GetCurrentMethod().GetParameters().Select(x => x.ParameterType);

        [HystrixInterceptor]
        public Task<object> MethodWithInterceptAttribute() => Task.FromResult(new object());
    }
}