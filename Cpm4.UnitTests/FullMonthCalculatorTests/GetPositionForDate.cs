using Cpm.Core.Services.Diary;
using Xunit;

namespace Cpm4.UnitTests.FullMonthCalculatorTests
{
    public class GetPositionForDate
    {
        [Theory]

        [InlineData("2018-01-01", "2018-01-01", 1)]
        [InlineData("2018-01-03", "2018-01-01", 1)]
        [InlineData("2017-12-29", "2018-01-01", 1)]

        [InlineData("2018-01-01", "2018-02-01", 2)]
        [InlineData("2018-01-03", "2018-02-01", 2)]
        [InlineData("2017-12-29", "2018-02-01", 2)]
        public void Returns_correct(
            string yearStartString,
            string requestedDateString,
            int expected
        )
        {
            var yearStart = Helpers.ToDate(yearStartString);
            var requestedDate = Helpers.ToDate(requestedDateString);
            var sut = new FullMonthCalculator();

            var actual = sut.GetPositionForDate(yearStart, requestedDate);

            Assert.Equal(expected, actual);
        }
    }
}
