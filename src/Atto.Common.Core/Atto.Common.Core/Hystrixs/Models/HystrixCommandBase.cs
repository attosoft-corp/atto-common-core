using Microsoft.Extensions.Logging;
using Steeltoe.CircuitBreaker.Hystrix;
using System;
using System.Linq;

namespace Atto.Common.Core.Hystrixs.Models
{
    public class HystrixCommandBase : HystrixCommand
    {
        private readonly Delegate _primaryDelegate;
        private readonly Delegate _fallbackDelegate;
        private readonly object[] _arguments;

        public HystrixCommandBase(IHystrixCommandOptions commandOptions, Delegate primary, Delegate fallback, object[] arguments, ILoggerFactory loggerFactory) :
            base(commandOptions: commandOptions, logger: loggerFactory.CreateLogger<HystrixCommandAsyncBase>())
        {
            _primaryDelegate = primary;
            _fallbackDelegate = fallback;
            _arguments = arguments;
            options.FallbackEnabled = fallback != null;
            IsFallbackUserDefined = fallback != null;
        }

        protected override void Run()
        {
            _primaryDelegate.DynamicInvoke(_arguments);
        }

        protected override void RunFallback()
        {
            var args = new object[] { new HystrixFallback() };

            args = args.Concat(_arguments).ToArray();

            _fallbackDelegate.DynamicInvoke(args);
        }
    }

    public class HystrixCommandBase<TResult> : HystrixCommand<TResult>
    {
        private readonly Delegate _primaryDelegate;
        private readonly Delegate _fallbackDelegate;
        private object[] _arguments;

        public HystrixCommandBase(IHystrixCommandOptions commandOptions, Delegate primary, Delegate fallback, object[] arguments, ILoggerFactory loggerFactory) :
             base(commandOptions: commandOptions, logger: loggerFactory.CreateLogger<HystrixCommandAsyncBase>())
        {
            _primaryDelegate = primary;
            _fallbackDelegate = fallback;
            _arguments = arguments;
            options.FallbackEnabled = fallback != null;
            IsFallbackUserDefined = fallback != null;
        }

        protected override TResult Run()
        {
            dynamic result = _primaryDelegate.DynamicInvoke(_arguments);
            return result;
        }

        protected override TResult RunFallback()
        {
            var args = new object[] { new HystrixFallback() };

            args = args.Concat(_arguments).ToArray();

            dynamic result = _fallbackDelegate.DynamicInvoke(args);

            args.Skip(1).ToArray().CopyTo(_arguments, 0);

            return result;
        }
    }
}