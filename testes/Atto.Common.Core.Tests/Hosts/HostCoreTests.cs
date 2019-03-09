using Atto.Common.Core.Extensions;
using Atto.Common.Core.Tests.Extensions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Xunit;

namespace Atto.Common.Core.Tests.Hosts
{
    public class HostCoreTests
    {
        [Fact]
        public void CreateWebHostBuilder_StateUnderTest_ExpectedBehavior()
        {
            var args = new string[] { };

            var param = new Dictionary<string, string>
            {
                { "spring:application:name", "ApplicationName" },
                { "spring:cloud:config:enable", "false" },
                { "eureka:client:shouldRegisterWithEureka", "false" },
                { "eureka:client:shouldFetchRegistry", "false" }
            };
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(param).Build();

            var webHost = WebHost.CreateDefaultBuilder(args)
                .UseAttoSoft()
                .UseConfiguration(configuration)
                .UseEnvironment("Development")
                .UseStartup<StartupTest>();

            using (var server = new TestServer(webHost))
            using (var client = server.CreateClient())
            {
            }
        }
    }
}