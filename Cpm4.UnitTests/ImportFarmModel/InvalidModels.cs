using Cpm.Core.Farms;
using Newtonsoft.Json;
using Xunit;

namespace Cpm4.UnitTests.ImportFarmModel
{
    public class InvalidModels
    {
        [Fact]
        public void Farm_with_no_name_causes_error()
        {
            var farm = Helper.CreateSimplestFarm();
            farm.Name = "";
            var json = JsonConvert.SerializeObject(farm);

            var sut = FarmModel.Parse(json);

            Assert.False(sut.Success);
            Assert.Contains(sut.Errors, e => e.Contains("farm name is empty"));
        }

        [Fact]
        public void Farm_with_no_sites_causes_error()
        {
            var farm = Helper.CreateSimplestFarm();
            farm.Sites = new SiteModel[] {};
            var json = JsonConvert.SerializeObject(farm);

            var sut = FarmModel.Parse(json);

            Assert.False(sut.Success);
            Assert.Contains(sut.Errors, e => e.Contains("no sites"));
        }

        [Fact]
        public void Site_with_no_name_causes_error()
        {
            var farm = Helper.CreateSimplestFarm();
            farm.Sites[0].Name = "";
            var json = JsonConvert.SerializeObject(farm);

            var sut = FarmModel.Parse(json);

            Assert.False(sut.Success);
            Assert.Contains(sut.Errors, e => e.Contains("site name is empty"));
        }

        [Fact]
        public void Site_with_no_fields_causes_error()
        {
            var farm = Helper.CreateSimplestFarm();
            farm.Sites[0].Fields = new FieldModel[] { };
            var json = JsonConvert.SerializeObject(farm);

            var sut = FarmModel.Parse(json);

            Assert.False(sut.Success);
            Assert.Contains(sut.Errors, e => e.Contains("no fields"));
        }

        [Fact]
        public void Field_with_no_name_causes_error()
        {
            var farm = Helper.CreateSimplestFarm();
            farm.Sites[0].Fields[0].Name = "";
            var json = JsonConvert.SerializeObject(farm);

            var sut = FarmModel.Parse(json);

            Assert.False(sut.Success);
            Assert.Contains(sut.Errors, e => e.Contains("field name is empty"));
        }

        [Fact]
        public void Field_with_zero_area_causes_error()
        {
            var farm = Helper.CreateSimplestFarm();
            farm.Sites[0].Fields[0].AreaInHectares = 0;
            var json = JsonConvert.SerializeObject(farm);

            var sut = FarmModel.Parse(json);

            Assert.False(sut.Success);
            Assert.Contains(sut.Errors, e => e.Contains("area should be > 0"));
        }
    }
}
