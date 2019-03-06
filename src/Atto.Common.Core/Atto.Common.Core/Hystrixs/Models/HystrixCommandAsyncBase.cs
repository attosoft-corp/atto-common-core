using Microsoft.Extensions.Logging;
using Steeltoe.CircuitBreaker.Hystrix;
using System;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace Atto.Common.Core.Hystrixs.Models
{
    public class HystrixCommandAsyncBase : HystrixCommand
    {
        private readonly Delegate _primaryDelegate;
        private readonly Delegate _fallbackDelegate;
        private readonly object[] _arguments;


        public HystrixCommandAsyncBase(IHystrixCommandOptions commandOptions, Delegate primary, Delegate fallback, object[] arguments, ILoggerFactory loggerFactory) :
             base(commandOptions: commandOptions, logger: loggerFactory.CreateLogger<HystrixCommandAsyncBase>())
        {
            _primaryDelegate = primary;
            _fallbackDelegate = fallback;
            _arguments = arguments;
            options.FallbackEnabled = fallback != null;
            IsFallbackUserDefined = fallback != null;
        }

        protected override Task<Unit> RunAsync()
        {
            _primaryDelegate.DynamicInvoke(_arguments);
            return Task.FromResult(Unit.Default);
        }

        protected override Task<Unit> RunFallbackAsync()
        {
            var args = new object[] { new HystrixFallback() };

            args = args.Concat(_arguments).ToArray();
            _ = _fallbackDelegate.DynamicInvoke(args);

            return Task.FromResult(Unit.Default);
        }
    }

    public class HystrixCommandAsyncBase<TResult> : HystrixCommand<TResult>
    {

        private readonly Delegate _primaryDelegate;
        private readonly Delegate _fallbackDelegate;
        private readonly object[] _arguments;

        public HystrixCommandAsyncBase(IHystrixCommandOptions commandOptions, Delegate primary, Delegate fallback, object[] arguments, ILoggerFactory loggerFactory) :
             base(commandOptions: commandOptions, logger: loggerFactory.CreateLogger<HystrixCommandAsyncBase>())
        {
            _primaryDelegate = primary;
            _fallbackDelegate = fallback;
            _arguments = arguments;
            options.FallbackEnabled = fallback != null;
            IsFallbackUserDefined = fallback != null;
        }

        protected override Task<TResult> RunAsync()
        {
            return (Task<TResult>)_primaryDelegate.DynamicInvoke(_arguments);
        }

        protected override Task<TResult> RunFallbackAsync()
        {
            var args = new object[] { new HystrixFallback() };

            args = args.Concat(_arguments).ToArray();

            return (Task<TResult>)_fallbackDelegate.DynamicInvoke(args);

        }
    }
}