using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore.Migrations;
using Newtonsoft.Json;
using Serilog.Events;
using Serilog.Sinks.AwsCloudWatch;

namespace Cpm.Web.Logging
{
    public class CustomLogEventRenderer : ILogEventRenderer
    {
        private static JsonSerializerSettings _serializerSettings;

        public CustomLogEventRenderer()
        {
            _serializerSettings = new JsonSerializerSettings
            {
                Culture = CultureInfo.CurrentCulture,
                Formatting = Formatting.Indented,
            };
        }

        public string RenderLogEvent(LogEvent logEvent)
        {
            var builder = new StringBuilder();

            builder.AppendFormat("[{0}] ", logEvent.Level.ToString().ToUpperInvariant().LimitLength(3));

            builder.Append(logEvent.RenderMessage(CultureInfo.CurrentCulture));

            if (logEvent.Properties.Any())
            {
                builder.AppendLine(JsonConvert.SerializeObject(
                    logEvent.Properties
                        .ToDictionary(k => k.Key, v =>
                        {
                            if (v.Value is ScalarValue scalar && scalar.Value is string s)
                            {
                                return s;
                            }
                            var x = new StringWriter(CultureInfo.CurrentCulture);
                            v.Value.Render(x, formatProvider: CultureInfo.CurrentCulture);
                            return x.ToString();
                        }),
                    _serializerSettings
                ));
            }

            if (logEvent.Exception != null)
            {
                builder.AppendLine("Exception:");
                builder.AppendLine(logEvent.Exception.ToString());
            }

            return builder.ToString();
        }
    }
}