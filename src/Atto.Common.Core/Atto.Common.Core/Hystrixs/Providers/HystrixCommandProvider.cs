using Atto.Common.Core.Hystrixs.Interface;
using Atto.Common.Core.Hystrixs.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Steeltoe.CircuitBreaker.Hystrix;
using Steeltoe.CircuitBreaker.Hystrix.Strategy;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Atto.Common.Core.Hystrixs
{
    public class HystrixCommandProvider : IHystrixCommandProvider
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IConfiguration _configuration;
        private readonly Type[] _contructorTypes;

        public HystrixCommandProvider(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            HystrixCommands = new ConcurrentDictionary<MethodInfo, IHystrixDefinition>();
            _loggerFactory = loggerFactory;
            _configuration = configuration;
            _contructorTypes = new Type[]
            {
                typeof(IHystrixCommandOptions),
                typeof(Delegate),
                typeof(Delegate),
                typeof(object[]),
                typeof(ILoggerFactory)
            };
        }

        private ConcurrentDictionary<MethodInfo, IHystrixDefinition> HystrixCommands { get; set; }

        public Task<object> ExecuteAsync(MethodInfo methodInfo, object target, params object[] args)
        {
            // If its already in the dictionary call it direct
            if (HystrixCommands.TryGetValue(methodInfo, out var def))
                return Task.FromResult(def.Execute(target, args, _loggerFactory));

            //Command options from configuration
            var hystrixCommandOptions = GetCommandOptions(methodInfo.ReflectedType.Name, methodInfo.Name);

            // Define hystrix definition call
            def = new HystrixDefinition(hystrixCommandOptions, methodInfo, target);

            // Get constructor from hystrix type definition
            var hystrixConstructorInfo = def.HystrixBaseType.GetConstructor(_contructorTypes);

            // Define parameters for lambida expression
            // (p0,p1,p2,p3,p4)
            var hystrixParametersConstructor = _contructorTypes.Select((type, i) => Expression.Parameter(type, $"p{i}")).ToArray();

            //Define the body of the expression to be executed
            //new HystrixCommand<?>(p0,p1,p2,p3,p4)
            var expressionNew = Expression.New(hystrixConstructorInfo, hystrixParametersConstructor);

            // Create a lambida with the parameters and body
            // (p0,p1,p2,p3,p4) new HystrixCommand<Type>(p0,p1,p2,p3,p4)
            var hystrixInstanceDelegate = Expression.Lambda(expressionNew, hystrixParametersConstructor);

            // Define the instance parameter to be passed in the lambida to execute the medhod of if
            // (hystrixCommand) =>
            var hystrixExpressionParameter = Expression.Parameter(def.HystrixBaseType, nameof(HystrixCommand).ToLower());

            var hystrixCommandMethodInfo = default(MethodInfo);
            var hystrixExpressionCall = default(MethodCallExpression);
            var hystrixLambidaMethod = default(LambdaExpression);

            if (def.IsTypeOfTask)
            {
                // Define parameter for cancelation token
                var cancelationTokenParameterExpression = Expression.Parameter(typeof(CancellationToken), nameof(CancellationToken).ToLower());

                // Get method that contains cancelation token
                hystrixCommandMethodInfo = def.HystrixBaseType.GetMethod(nameof(HystrixCommand.ExecuteAsync), new Type[] { typeof(CancellationToken) });

                // Define a call to method
                // hystrixCommand.ExecuteAsync(cancelationToken)
                hystrixExpressionCall = Expression.Call(hystrixExpressionParameter, hystrixCommandMethodInfo, cancelationTokenParameterExpression);

                // Create the lambida to execute the method.
                // (hystrixCommand,cancelationToken) => hystrixCommand.ExecuteAsync(cancelationToken)
                hystrixLambidaMethod = Expression.Lambda(hystrixExpressionCall, hystrixExpressionParameter, cancelationTokenParameterExpression);
            }
            else
            {
                // Get method that will be executed.
                hystrixCommandMethodInfo = def.HystrixBaseType.GetMethod(nameof(HystrixCommand.Execute), new Type[] { });

                // Define a call to method
                // hystrixCommand.Execute()
                hystrixExpressionCall = Expression.Call(hystrixExpressionParameter, hystrixCommandMethodInfo);

                // Create the lambida to execute the method.
                // (hystrixCommand) => hystrixCommand.Execute()
                hystrixLambidaMethod = Expression.Lambda(hystrixExpressionCall, hystrixExpressionParameter);
            }

            // compile the definitions
            def.HystrixInstanceDelegate = hystrixInstanceDelegate.Compile();
            def.HystrixMethodDelegate = hystrixLambidaMethod.Compile();

            //Store in dictionary to be executed later
            if (HystrixCommands.TryAdd(def.HystrixMethodInfo, def))
                return ExecuteAsync(methodInfo, target, args);
            else
                throw new Exception("Could not store the definition of hystrix");
        }

        private IHystrixCommandOptions GetCommandOptions(string serviceName, string methodName)
        {
            var strategy = HystrixPlugins.OptionsStrategy;
            var dynOpts = strategy.GetDynamicOptions(_configuration);

            var commandKey = HystrixCommandKeyDefault.AsKey($"{serviceName}.{methodName}");
            var groupKey = HystrixCommandGroupKeyDefault.AsKey($"{serviceName}Group");

            IHystrixCommandOptions opts = new HystrixCommandOptions(groupKey, commandKey, null, dynOpts)
            {
                ThreadPoolKey = HystrixThreadPoolKeyDefault.AsKey($"{serviceName}Group")
            };
            opts.ThreadPoolOptions = new HystrixThreadPoolOptions(opts.ThreadPoolKey, null, dynOpts);

            return opts;
        }
    }
}