using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Cpm.Core.Services.Serialization
{
    public class JsonWeightsSerializer : IWeightsSerializer
    {
        public string Serialize(IEnumerable<decimal?> values)
        {
            return JsonConvert.SerializeObject(values);
        }

        public IEnumerable<decimal?> Deserialize(string text)
        {
            return string.IsNullOrEmpty(text)
                ? Enumerable.Empty<decimal?>()
                : JsonConvert.DeserializeObject<decimal?[]>(text);
        }
    }
}