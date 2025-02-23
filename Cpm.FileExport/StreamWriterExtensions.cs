using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cpm.FileExport
{
    internal static class StreamWriterExtensions
    {
        public static void WriteCsvLine(this StreamWriter writer, IEnumerable<string> columns)
        {
            writer.WriteLine(FormatCsvLine(columns));
        }

        private static string FormatCsvLine(IEnumerable<string> columns)
        {
            return string.Join(",", columns.Select(FormatCsvValue));
        }

        private static string FormatCsvValue(string value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            if (MustEscape(value))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }

            return value;
        }

        private static bool MustEscape(string value)
        {
            return value.Contains("\"") || value.Contains(",") || value.Contains("\n") || value.Contains("\r");
        }
    }
}