using Atto.Common.Core.Extensions;
using Atto.Common.Core.Program;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Atto.Common.Core.Tests.Extensions
{
    public class SwaggerExtensionTests
    {


        public SwaggerExtensionTests()
        {

        }


        [Fact]
        public async Task TestSwagger()
        {
            var args = new string[] { };
            string contentRoot = Directory.GetCurrentDirectory();

            var param = new Dictionary<string, string>
            {
                { "spring:application:name", "ApplicationName" },
                { "spring:cloud:config:enable", "false" },
                { "eureka:client:shouldRegisterWithEureka", "false" },
                { "eureka:client:shouldFetchRegistry", "false" }
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(param).Build();


            var webHost = HostCore.CreateWebHostBuilder(args, contentRoot)
                .UseConfiguration(configuration)
                .UseEnvironment("SwaggerTest")
                .UseStartup<StartupTest>();

            using (var server = new TestServer(webHost))
            using (var client = server.CreateClient())
            {
                string result = await client.GetStringAsync("/swagger/v1/swagger.json");
                JObject root = JObject.Parse(result);
            }
        }
    }
}
