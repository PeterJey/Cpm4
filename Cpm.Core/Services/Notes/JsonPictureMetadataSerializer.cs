using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cpm.Core.Services.Notes
{
    public class JsonPictureMetadataSerializer : IPictureMetadataSerializer
    {
        public string Serialize(IEnumerable<PictureMetadata> pictures)
        {
            return JsonConvert.SerializeObject(pictures);
        }

        public List<PictureMetadata> Deserialize(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return new List<PictureMetadata>();
            }

            return JsonConvert.DeserializeObject<List<PictureMetadata>>(text);
        }
    }
}