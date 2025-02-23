using Cpm.Core.Services.Forecast;
using Newtonsoft.Json;

namespace Cpm.Core.Services.Serialization
{
    public class JsonYieldSerializer : IYieldSerializer
    {
        private readonly JsonSerializerSettings _jsonSettings;

        public JsonYieldSerializer()
        {
            _jsonSettings = new JsonSerializerSettings
            {
                Converters =
                {
                    new JsonYieldConverter()
                }
            };
        }

        public string Serialize(Yield yield)
        {
            return JsonConvert.SerializeObject(yield, _jsonSettings);
        }

        public Yield Deserialize(string text)
        {
            return string.IsNullOrEmpty(text)
                ? new YieldPerHectare(0)
                : JsonConvert.DeserializeObject<Yield>(text, _jsonSettings);
        }
    }
}