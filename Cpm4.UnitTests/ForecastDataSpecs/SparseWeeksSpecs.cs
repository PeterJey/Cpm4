using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cpm.Core.Services.Forecast;
using Xunit;

namespace Cpm4.UnitTests.ForecastDataSpecs
{
    public class SparseWeeksSpecs
    {
        [Fact]
        public void WhenThereIsAGap_GetWeeks_FillsBlanks()
        {
            var values = new[]
            {
                new ForecastValue
                {
                    Weight = 1
                },
                new ForecastValue
                {
                    Weight = 2
                },
            };
            var sut = new ForecastData(5, values);
            sut.Override(9, new [] { 5m, 6m }, c => new ForecastValue { Weight = c.Factor } );

            var actual = sut.GetWeeks().ToArray();

            Assert.Equal(6, actual.Length);
            Assert.Equal(new[] { 1m, 2m, 0m, 0m, 5m, 6m }, actual.Select(x => x.Weight));
            Assert.Equal(new[]
            {
                HarvestValueType.Forecasted,
                HarvestValueType.Forecasted,
                HarvestValueType.Inferred,
                HarvestValueType.Inferred,
                HarvestValueType.Forecasted,
                HarvestValueType.Forecasted,
            }, actual.Select(x => x.Type));
        }
    }
}
