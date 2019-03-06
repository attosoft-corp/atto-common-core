using Atto.Common.Core.Hystrixs.Interface;
using Dora.DynamicProxy;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Threading.Tasks;

namespace Atto.Common.Core.Interceptors
{
    public class HystrixInterceptor
    {
        private readonly IHystrixCommandProvider _hystrixCommandProvider;
        private readonly ILogger _logger;

        public HystrixInterceptor(ILoggerFactory loggerFactory, IHystrixCommandProvider hystrixCommandProvider)
        {
            _hystrixCommandProvider = hystrixCommandProvider;
            _logger = loggerFactory.CreateLogger<HystrixInterceptor>();
        }

        public async Task InvokeAsync(InvocationContext context)
        {
            var methodInfo = context.TargetMethod as MethodInfo;
            using (var scope = _logger.BeginScope("begin hystrix command for class {0} and method:{1}", methodInfo.ReflectedType, methodInfo.Name))
            {
                var result = _hystrixCommandProvider.ExecuteAsync(methodInfo, context.Target, context.Arguments);
                context.ReturnValue = await result;
            }
        }
    }
}