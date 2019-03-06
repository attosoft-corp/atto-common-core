using Atto.Common.Core.Program;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using System;
using System.IO;
using Xunit;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.Threading.Tasks;
using Steeltoe.CircuitBreaker.Hystrix;
using Microsoft.AspNetCore.TestHost;

namespace Atto.Common.Core.Tests.Extensions
{
    public class ServiceCollectionExtensionsTests
    {

        [Fact]
        public void AddDefaultCollections_StateUnderTest_ExpectedBehavior()
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
                .UseEnvironment("Development")
                .UseStartup<StartupTest>();

            using (var server = new TestServer(webHost))
            using (var client = server.CreateClient())
            {
                
            }

        }
    }

}
