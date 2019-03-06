using Atto.Common.Core.Hystrixs.Models;
using Xunit;

namespace Atto.Common.Core.Tests.Hystrixs.Models
{
    public class HystrixFallbackTests
    {
        public HystrixFallbackTests()
        {
        }

        private HystrixFallback CreateHystrixFallback()
        {
            return new HystrixFallback();
        }

        [Fact]
        public void TestMethod1()
        {
            // Arrange
            var unitUnderTest = this.CreateHystrixFallback();

            // Act

            // Assert
            Assert.True(true);
        }
    }
}