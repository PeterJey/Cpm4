using System.Collections.Generic;

namespace Cpm.Core.Services.Forecast
{
    public interface IAlgorithmSettingsSerializer
    {
        IDictionary<string, string> Deserialize(string text);
        string Serialize(IDictionary<string, string> settings);
    }
}