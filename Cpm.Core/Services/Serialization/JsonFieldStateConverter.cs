using System;
using System.Collections.Generic;
using Cpm.Core.Services.Scenarios;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cpm.Core.Services.Serialization
{
    public class JsonFieldStateConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(FieldState));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);

            var settings = jo["Settings"].ToObject<Dictionary<string, string>>();
            var isVisible = jo["IsVisible"].ToObject<bool>();
            string algorithm = jo["Algorithm"].ToObject<string>();

            var result = new FieldState(settings, algorithm, isVisible);

            return result;
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}