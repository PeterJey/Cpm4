using Cpm.Core.Services.Forecast;
using Cpm.Core.Services.Serialization;
using Newtonsoft.Json;
using Xunit;

namespace Cpm4.UnitTests.JsonYieldConverterTests
{
    public class Serialize
    {
        [Fact]
        public void For_per_hectare_returns_correct_value()
        {
            var kgPerHectare = 1234m;
            var yield = new YieldPerHectare(kgPerHectare);
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                Converters =
                {
                    new JsonYieldConverter(),
                },
            };
            var expected = JsonConvert.SerializeObject(new { KgPerHectare = kgPerHectare }, Formatting.None);

            var actual = JsonConvert.SerializeObject(yield, settings);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void For_per_plant_returns_correct_value()
        {
            var gramsPerPlant = 345m;
            var plantsPerHectare = 789;
            var yield = new YieldPerPlant(gramsPerPlant, plantsPerHectare);
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                Converters =
                {
                    new JsonYieldConverter(),
                },
            };
            var expected = JsonConvert.SerializeObject(new { GramsPerPlant = gramsPerPlant, PlantsPerHectare = plantsPerHectare }, Formatting.None);

            var actual = JsonConvert.SerializeObject(yield, settings);

            Assert.Equal(expected, actual);
        }
    }
}
