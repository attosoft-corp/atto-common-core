using System.Reflection;
using System.Threading.Tasks;

namespace Atto.Common.Core.Hystrixs.Interface
{
    public interface IHystrixCommandProvider
    {
        Task<object> ExecuteAsync(MethodInfo methodInfo, object target, object[] args);
    }
}