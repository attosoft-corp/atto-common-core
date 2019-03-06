using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Atto.Common.Core.Tests.Helpers
{
    public class TestAutoConfig 
    {

        public IServiceCollection ConfigureServices(IServiceCollection services)
        {
            return services;
        }
    }
}
