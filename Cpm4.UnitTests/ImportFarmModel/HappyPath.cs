using Cpm.Core.Farms;
using Newtonsoft.Json;
using Xunit;

namespace Cpm4.UnitTests.ImportFarmModel
{
    public class HappyPath
    {
        [Fact]
        public void Farm_with_one_site_and_field_success()
        {
            var farm = Helper.CreateSimplestFarm();
            var json = JsonConvert.SerializeObject(farm);

            var sut = FarmModel.Parse(json);

            Assert.True(sut.Success);
        }

        [Fact]
        public void Farm_with_two_sites_have_two_sites()
        {
            var farm = Helper.CreateSimplestFarm();
            var site = farm.Sites[0];
            farm.Sites = new[] { site, site };
            var json = JsonConvert.SerializeObject(farm);

            var sut = FarmModel.Parse(json);

            Assert.True(sut.Success);
            Assert.Equal(2, sut.Model.Sites.Length);
        }

        [Fact]
        public void Site_with_two_fields_have_two_fields()
        {
            var farm = Helper.CreateSimplestFarm();
            var field = farm.Sites[0].Fields[0];
            farm.Sites[0].Fields = new[] { field, field };
            var json = JsonConvert.SerializeObject(farm);

            var sut = FarmModel.Parse(json);

            Assert.True(sut.Success);
            Assert.Equal(2, sut.Model.Sites[0].Fields.Length);
        }
    }
}