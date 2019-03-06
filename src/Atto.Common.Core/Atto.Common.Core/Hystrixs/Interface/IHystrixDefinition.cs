using Atto.Common.Core.Hystrixs.Models;
using Microsoft.Extensions.Logging;
using Steeltoe.CircuitBreaker.Hystrix;
using System;
using System.Reflection;

namespace Atto.Common.Core.Hystrixs.Interface
{
    public interface IHystrixDefinition
    {
        Type HystrixBaseType { get; }
        IHystrixCommandOptions HystrixCommandOptions { get; }
        Delegate HystrixInstanceDelegate { get; set; }
        Delegate HystrixMethodDelegate { get; set; }
        MethodInfo HystrixMethodInfo { get; }
        bool IsTypeOfTask { get; }
        DelegateContext PrimaryDelegateContext { get; set; }
        DelegateContext FallbackDelegateContext { get; set; }

        object Execute(object target, object[] args, ILoggerFactory loggerFactory);
    }
}