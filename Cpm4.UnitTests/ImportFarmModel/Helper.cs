using Cpm.Core.Farms;

namespace Cpm4.UnitTests.ImportFarmModel
{
    public class Helper
    {
        public static FarmModel CreateSimplestFarm()
        {
            var field = new FieldModel
            {
                Name = "fl",
                AreaInHectares = 1m
            };
            var site = new SiteModel
            {
                Name = "s",
                Fields = new[]
                {
                    field,
                    field
                }
            };
            var farm = new FarmModel
            {
                Name = "f",
                Sites = new[]
                {
                    site
                }
            };
            return farm;
        }
    }
}