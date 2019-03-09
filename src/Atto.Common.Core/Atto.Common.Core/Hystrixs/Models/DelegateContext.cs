using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Atto.Common.Core.Hystrixs.Models
{
    public class DelegateContext
    {
        private MethodInfo _methodInfo;
        private Type _delegateType;
        private readonly Type[] _fallbackParameters;

        public DelegateContext(MethodInfo methodInfo, params Type[] fallbackParam)
        {
            _methodInfo = methodInfo;
            _fallbackParameters = fallbackParam;
            DefinePropertiesDelegate();
        }

        private void DefinePropertiesDelegate()
        {
            var types = _methodInfo.GetParameters().Select(p => p.ParameterType);
            types = _fallbackParameters.Concat(types);

            _methodInfo = _methodInfo.ReflectedType.GetMethod(_methodInfo.Name, types.ToArray());
            if (_methodInfo == null) return;

            Func<Type[], Type> getType = Expression.GetActionType;
            var isAction = _methodInfo.ReturnType.Equals(typeof(void));

            if (!isAction)
            {
                getType = Expression.GetDelegateType;
                types = types.Concat(new[] { _methodInfo.ReturnType });
            }

            _delegateType = getType(types.ToArray());
        }

        public Delegate CreateDelegate(object target = default)
        {
            if (_methodInfo == null) return null;
            if (_methodInfo.IsStatic) return Delegate.CreateDelegate(_delegateType, _methodInfo);

            return Delegate.CreateDelegate(_delegateType, target, _methodInfo.Name);
        }
    }
}