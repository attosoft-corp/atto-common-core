using Atto.Common.Core.Hystrixs.Interface;
using Microsoft.Extensions.Logging;
using Steeltoe.CircuitBreaker.Hystrix;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Atto.Common.Core.Hystrixs.Models
{
    public class HystrixDefinition : IHystrixDefinition
    {
        public HystrixDefinition(IHystrixCommandOptions commandOptions, MethodInfo methodInfo, object target)
        {
            HystrixCommandOptions = commandOptions ?? throw new ArgumentNullException(nameof(commandOptions));
            HystrixMethodInfo = methodInfo ?? throw new ArgumentNullException(nameof(methodInfo));
            if (target == null) throw new ArgumentNullException(nameof(target));

            if (methodInfo.ReturnType.IsGenericType)
                IsTypeOfTask = methodInfo.ReturnType.GetGenericTypeDefinition() == typeof(Task<>) ||
                    methodInfo.ReturnType.GetGenericTypeDefinition() == typeof(Task);

            if (methodInfo.ReturnType == typeof(Task)) IsTypeOfTask = true;

            HystrixBaseType = DefineHystrixBaseType();
            PrimaryDelegateContext = new DelegateContext(methodInfo);
            FallbackDelegateContext = new DelegateContext(methodInfo, typeof(HystrixFallback));
        }

        public Delegate HystrixInstanceDelegate { get; set; }

        public Delegate HystrixMethodDelegate { get; set; }

        public DelegateContext PrimaryDelegateContext { get; set; }

        public DelegateContext FallbackDelegateContext { get; set; }

        public bool IsTypeOfTask { get; private set; }

        public IHystrixCommandOptions HystrixCommandOptions { get; private set; }

        public MethodInfo HystrixMethodInfo { get; private set; }

        public Type HystrixBaseType { get; private set; }

        private Type DefineHystrixBaseType()
        {
            var returnType = HystrixMethodInfo.ReturnType;

            if (HystrixMethodInfo.ReturnType.IsGenericType)
            {
                var genericTypeDefinition = HystrixMethodInfo.ReturnType.GetGenericTypeDefinition();

                if (genericTypeDefinition == typeof(Task<>))
                {
                    var genericReturnType = HystrixMethodInfo.ReturnType.GenericTypeArguments.LastOrDefault();
                    return typeof(HystrixCommandAsyncBase<>).MakeGenericType(genericReturnType);
                }
                return typeof(HystrixCommandBase<>).MakeGenericType(returnType);
            }
            else
            {
                if (returnType == typeof(void))
                {
                    return typeof(HystrixCommandBase);
                }
                if (returnType == typeof(Task))
                {
                    return typeof(HystrixCommandAsyncBase);
                }
            }

            return typeof(HystrixCommandBase<>).MakeGenericType(returnType);
        }

        public object Execute(object target, object[] args, ILoggerFactory loggerFactory)
        {
            var primary = PrimaryDelegateContext.CreateDelegate(target);
            var fallback = FallbackDelegateContext.CreateDelegate(target);

            var instance = HystrixInstanceDelegate.DynamicInvoke(HystrixCommandOptions, primary, fallback, args, loggerFactory);
            var token = args.OfType<CancellationToken>().FirstOrDefault();
            token = token == null ? CancellationToken.None : token;

            if (IsTypeOfTask)
                return HystrixMethodDelegate.DynamicInvoke(instance, token);
            else
                return HystrixMethodDelegate.DynamicInvoke(instance);
        }
    }
}