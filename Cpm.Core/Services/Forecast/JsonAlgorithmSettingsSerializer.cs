using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cpm.Core.Services.Forecast
{
    public class JsonAlgorithmSettingsSerializer : IAlgorithmSettingsSerializer
    {
        public IDictionary<string, string> Deserialize(string text)
        {
            return EnsureDefaults(JsonConvert.DeserializeObject<Dictionary<string, string>>(text));
        }

        public string Serialize(IDictionary<string, string> settings)
        {
            return JsonConvert.SerializeObject(settings);
        }

        private IDictionary<string, string> EnsureDefaults(IDictionary<string, string> dict)
        {
            if (dict == null)
            {
                dict = new Dictionary<string, string>();
            }

            dict.TryAdd("WeekOffset", "0");

            return dict;
        }
    }
}