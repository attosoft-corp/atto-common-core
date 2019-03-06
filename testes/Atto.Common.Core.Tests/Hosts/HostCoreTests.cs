using Atto.Common.Core.Extensions;
using Atto.Common.Core.Program;
using Atto.Common.Core.Tests.Extensions;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Atto.Common.Core.Tests.Hosts
{
    public class HostCoreTests
    {

        [Fact]
        public void CreateWebHostBuilder_StateUnderTest_ExpectedBehavior()
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
