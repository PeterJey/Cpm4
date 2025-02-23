namespace Cpm.Core.Services.Forecast
{
    public interface ICorrectingStrategy
    {
        decimal Correct(decimal value, decimal factor, decimal totalDelta);
    }
}