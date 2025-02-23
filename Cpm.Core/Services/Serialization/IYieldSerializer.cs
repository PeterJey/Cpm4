using Cpm.Core.Services.Forecast;

namespace Cpm.Core.Services.Serialization
{
    public interface IYieldSerializer
    {
        string Serialize(Yield yield);
        Yield Deserialize(string text);
    }
}