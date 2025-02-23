using System;
using Optional;

namespace Cpm.Core.Services.Forecast
{
    public class YieldFactory
    {
        public static bool TryCreate(IUserPreferences preferences, string ypp, string ppa, string ypa, out Yield yield)
        {
            var yieldPerPlant = ParseNumber(ypp);
            var plantsPerArea = ParseNumber(ppa);
            var yieldPerArea = ParseNumber(ypa);

            var perPlant = yieldPerPlant
                .FlatMap(y => plantsPerArea.Map(p => (Yield)new YieldPerPlant(y, (int)Math.Round(preferences.FromAreaUnit(p)))));
            var perArea = yieldPerArea
                .Map(x => (Yield)new YieldPerHectare(preferences.FromAreaUnit(x)));

            yield = perPlant
                .Else(perArea
                        .Else(perArea))
                .ValueOr((Yield)null);

            return yield != null;
        }

        private static Option<decimal> ParseNumber(string text)
        {
            return decimal.TryParse(text, out var value) 
                ? value.Some() 
                : Option.None<decimal>();
        }
    }
}