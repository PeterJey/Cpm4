namespace Cpm.Core.Services.Forecast
{
    public class YieldPerHectare : Yield
    {
        public override decimal KgPerHectare { get; }

        public YieldPerHectare(decimal kgPerHectare)
        {
            KgPerHectare = kgPerHectare;
        }

        public override string ToString(decimal areaFactor, string areaUnit)
        {
            return $"{(KgPerHectare * areaFactor):N0} kg/{areaUnit}";
        }

        public override Yield ScaleBy(decimal scale)
        {
            return new YieldPerHectare(KgPerHectare * scale);
        }
    }
}