using Testing;
using Xunit;
using Xunit.Abstractions;

namespace Data.Dynamic.Tests
{
    public class DynamicDataExtensionsTests : TestBase<dynamic>
    {
        public DynamicDataExtensionsTests(ITestOutputHelper console) : base(console)
        {
        }

        [Fact]
        public void ItShouldInstantiate()
        {
            Assert.NotNull(Unit);
        }
    }
}