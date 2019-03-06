using Atto.Common.Core.Interceptors;
using Dora.Interception;

namespace Atto.Common.Core.Attributes
{
    public class HystrixInterceptorAttribute : InterceptorAttribute
    {
        public override void Use(IInterceptorChainBuilder builder)
        {
            builder.Use<HystrixInterceptor>(Order);
        }
    }
}