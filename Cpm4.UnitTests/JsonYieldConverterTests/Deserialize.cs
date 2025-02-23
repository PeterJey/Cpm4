using Cpm.Core.Services.Forecast;
using Cpm.Core.Services.Serialization;
using Newtonsoft.Json;
using Xunit;

namespace Cpm4.UnitTests.JsonYieldConverterTests
{
    public class Deserialize
    {
        [Fact]
        public void For_per_hectare_returns_correct_value()
        {
            var kgPerHectare = 1234m;
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                Converters =
                {
                    new JsonYieldConverter(),
                },
            };
            var serialized = JsonConvert.SerializeObject(new { KgPerHectare = kgPerHectare }, Formatting.None);

            var actual = JsonConvert.DeserializeObject<Yield>(serialized, settings);

            Assert.IsType<YieldPerHectare>(actual);
            Assert.Equal(kgPerHectare, (actual as YieldPerHectare).KgPerHectare);
        }

        [Fact]
        public void For_per_plant_returns_correct_value()
        {
            var gramsPerPlant = 456m;
            var plantsPerHectare = 789;
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                Converters =
                {
                    new JsonYieldConverter(),
                },
            };
            var serialized = JsonConvert.SerializeObject(new { GramsPerPlant = gramsPerPlant, PlantsPerHectare = plantsPerHectare }, Formatting.None);

            var actual = JsonConvert.DeserializeObject<Yield>(serialized, settings);

            Assert.IsType<YieldPerPlant>(actual);
            Assert.Equal(gramsPerPlant, (actual as YieldPerPlant).GramsPerPlant);
            Assert.Equal(plantsPerHectare, (actual as YieldPerPlant).PlantsPerHectare);
        }
    }
}
