namespace Cpm.Core.Services.Forecast
{
    public class YieldPerPlant : Yield
    {
        public decimal GramsPerPlant { get; }
        public int PlantsPerHectare { get; }

        public override decimal KgPerHectare => GramsPerPlant * PlantsPerHectare / 1000;

        public YieldPerPlant(decimal gramsPerPlant, int plantsPerHectare)
        {
            GramsPerPlant = gramsPerPlant;
            PlantsPerHectare = plantsPerHectare;
        }

        public override string ToString(decimal areaFactor, string areaUnit)
        {
            return $"{GramsPerPlant:N0} g/plant";
        }

        public override Yield ScaleBy(decimal scale)
        {
            return new YieldPerPlant(GramsPerPlant * scale, PlantsPerHectare);
        }
    }
}