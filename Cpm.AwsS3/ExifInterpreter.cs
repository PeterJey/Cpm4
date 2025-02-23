using System;
using System.Globalization;
using Cpm.Core.Services.Notes;
using SixLabors.ImageSharp.MetaData.Profiles.Exif;

namespace Cpm.AwsS3
{
    public class ExifInterpreter
    {
        public static void PopulateMetadata(PictureMetadata metadata, ExifProfile exif)
        {
            if (exif == null)
            {
                return;
            }

            metadata.Latitude =
                DecodeCoordinate(exif.GetValue(ExifTag.GPSLatitudeRef), exif.GetValue(ExifTag.GPSLatitude));

            metadata.Longitude =
                DecodeCoordinate(exif.GetValue(ExifTag.GPSLongitudeRef), exif.GetValue(ExifTag.GPSLongitude));

            if (DateTime.TryParseExact(
                ((exif.GetValue(ExifTag.DateTimeDigitized)?.Value ?? exif.GetValue(ExifTag.DateTimeOriginal)?.Value) ?? "")?.ToString(),
                "yyyy:MM:dd HH:mm:ss",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var takenOn
            ))
            {
                metadata.TakenOn = takenOn;
            }
            else
            {
                metadata.TakenOn = null;
            }
        }

        private static decimal? DecodeCoordinate(ExifValue reference, ExifValue value)
        {
            if (reference == null || value == null || value.DataType != ExifDataType.Rational || !value.IsArray)
            {
                return null;
            }

            var r = reference.ToString().ToUpperInvariant();

            return DecodeRationalValues(value.Value as Rational[]) * (r == "W" || r == "S" ? -1 : 1);
        }

        private static decimal? DecodeRationalValues(Rational[] rational)
        {
            if (rational == null) return null;
            return RationalToDecimal(rational[0]) +
                   RationalToDecimal(rational[1]) / 60 +
                   RationalToDecimal(rational[2]) / 3600;
        }

        private static decimal RationalToDecimal(Rational rat) => (decimal)rat.Numerator / rat.Denominator;
    }
}