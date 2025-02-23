namespace Cpm.Core.Services.Forecast
{
    public abstract class Yield
    {
        public abstract decimal KgPerHectare { get; }

        public override string ToString()
        {
            return ToString(1m, "ha");
        }

        public abstract string ToString(decimal areaFactor, string areaUnit);

        public abstract Yield ScaleBy(decimal scale);
    }
}