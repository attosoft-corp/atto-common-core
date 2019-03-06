using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Atto.Common.Core.Extensions
{
    internal static class AutoConfigureExtension
    {
        private static readonly Func<IEnumerable<Type>> criteria = () => AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes().Where(t => t.Name.EndsWith("AutoConfig") && !t.IsInterface && !t.IsAbstract)).ToList();

        public static IServiceCollection AddAutoConfigure(this IServiceCollection services)
        {
            var autoConfigConventions = criteria().ToList();
            var serviceProvider = services.BuildServiceProvider();

            autoConfigConventions.ForEach(type =>
            {
                var instance = ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, type);
                var paramType = new Type[] { typeof(IServiceCollection) };
                var method = instance.GetType().GetMethod(nameof(IStartup.ConfigureServices), paramType);
                if (method != null)
                {
                    var paramFunc = paramType.Concat(new Type[] { method.ReturnType }).ToArray();
                    var configDelegate = Delegate.CreateDelegate(Expression.GetFuncType(paramFunc), instance, method);
                    services = (IServiceCollection)configDelegate.DynamicInvoke(services); 
                }
            });
            return services;
        }

        public static void UseAutoConfigure(this IApplicationBuilder app)
        {
            var autoConfigConventions = criteria().ToList();
            var serviceProvider = app.ApplicationServices.GetRequiredService<IServiceProvider>();
            var appbuilderArray = new object[] { app };
            autoConfigConventions.ForEach(type =>
            {
                var instance = ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, type);
                var methods = instance.GetType().GetMethods().Where(x => x.Name == nameof(IStartup.Configure)).ToList();

                methods.ForEach(method =>
                {
                    var types = method.GetParameters().Select(param => param.ParameterType).ToArray();

                    var objParam = types.Where(paramType => paramType != typeof(IApplicationBuilder))
                    .Select(paramType => ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, paramType)).ToArray();

                    var configDelegate = Delegate.CreateDelegate(Expression.GetActionType(types), instance, method);
                    configDelegate.DynamicInvoke(appbuilderArray.Concat(objParam).ToArray());
                });
            });

        }
    }


}