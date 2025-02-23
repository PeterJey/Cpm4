using System.Collections.Generic;
using Cpm.Core.Services.Profiles;
using Cpm.Core.Services.Scenarios;
using Newtonsoft.Json;

namespace Cpm.Infrastructure
{
    public class JsonCriteriaSerializer
    {
        public static JsonCriteriaSerializer Instance = new JsonCriteriaSerializer();

        private JsonCriteriaSerializer()
        {
        }

        public SeasonsProfile Deserialize(string text)
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<Season, string>>(text);

            return dict == null ? SeasonsProfile.Empty : SeasonsProfile.FromDictionary(dict);
        }

        public string Serialize(SeasonsProfile criteria)
        {
            return JsonConvert.SerializeObject(criteria.ToCompactDictionary());
        }
    }
}