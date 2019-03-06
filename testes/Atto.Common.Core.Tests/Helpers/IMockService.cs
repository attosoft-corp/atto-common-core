using Atto.Common.Core.Hystrixs.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Atto.Common.Core.Tests.Helpers
{
    public interface IMockService
    {
        object MethodRetunsObject();

        void MethodRetunsVoid(HystrixFallback hystrixFallback);

        Task<object> MethodRetunsTaskObject();

        void MethodRetunsVoid();

        Task MethodReturnsTask();

        Task MethodWithCancelationTokenTest(CancellationToken token);

        Task MethodWithCancelationTokenTest(HystrixFallback hystrixFallback, CancellationToken token);

        Task<IEnumerable<Type>> MethodWithExceptionTestAsync(HystrixFallback hystrixFallback, long a, bool b);

        Task<IEnumerable<Type>> MethodWithExceptionTestAsync(long a, bool b);

        Task<IEnumerable<Type>> MethodWithExceptionWithoutFallbackTestAsync();

        Task<IEnumerable<Type>> MethodWithoutParametersTestAsync();

        Task<IEnumerable<Type>> MethodWithoutParametersTestAsync(HystrixFallback hystrixFallback);

        Task<IEnumerable<Type>> MethodWithParametersTestAsync(HystrixFallback hystrixFallback, int a, int b);

        Task<IEnumerable<Type>> MethodWithParametersTestAsync(int a, string b);

        IEnumerable<Type> MethodWithTimeoutTest(HystrixFallback hystrixFallback, int a);

        IEnumerable<Type> MethodWithTimeoutTest(int a);

        Task<IEnumerable<Type>> MethodWithTimeoutTestAsync(HystrixFallback hystrixFallback, int a);

        Task<IEnumerable<Type>> MethodWithTimeoutTestAsync(int a);

        Task<object> MethodWithInterceptAttribute();
    }
}