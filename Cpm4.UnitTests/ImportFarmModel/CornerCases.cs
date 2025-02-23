using Cpm.Core.Farms;
using Xunit;

namespace Cpm4.UnitTests.ImportFarmModel
{
    public class CornerCases
    {
        [Fact]
        public void No_object_fails()
        {
            var json = "";

            var sut = FarmModel.Parse(json);

            Assert.False(sut.Success);
        }

        [Fact]
        public void Empty_object_fails()
        {
            var json = "{}";

            var sut = FarmModel.Parse(json);

            Assert.False(sut.Success);
        }

        [Fact]
        public void No_known_properties_fails()
        {
            var json = "{\"a\":\"1\"}";

            var sut = FarmModel.Parse(json);

            Assert.False(sut.Success);
        }
    }
}
