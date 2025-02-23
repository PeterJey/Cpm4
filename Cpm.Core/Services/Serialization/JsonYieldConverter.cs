using System;
using System.Collections.Generic;
using System.Linq;
using Cpm.Core.Services.Forecast;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cpm.Core.Services.Serialization
{
    public class JsonYieldConverter : JsonConverter
    {
        private readonly List<Type> _types = new List<Type> { typeof(YieldPerHectare), typeof(YieldPerPlant), typeof(Yield) };

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value.GetType() == typeof(YieldPerHectare))
            {
                JToken.FromObject(value).WriteTo(writer);
            }
            else
            {
                var token = JToken.FromObject(value);
                token["KgPerHectare"].Parent.Remove();
                token.WriteTo(writer);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType != typeof(Yield))
            {
                return null;
            }

            var obj = JObject.Load(reader);

            if (obj.Properties().Any(x => x.Name == "PlantsPerHectare") &&
                obj.Properties().Any(x => x.Name == "GramsPerPlant"))
            {
                var gramsPerPlant = (decimal)obj.Property(nameof(YieldPerPlant.GramsPerPlant)).Value.ToObject(typeof(decimal));
                var plantsPerHectare = (int)obj.Property(nameof(YieldPerPlant.PlantsPerHectare)).Value.ToObject(typeof(int));
                return new YieldPerPlant(gramsPerPlant, plantsPerHectare);
            }
            else
            {
                var kgPerHectare = (decimal)obj.Property(nameof(YieldPerHectare.KgPerHectare)).Value.ToObject(typeof(decimal));
                return new YieldPerHectare(kgPerHectare);
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return _types.Contains(objectType);
        }
    }
}