using Cpm.Core.Services.Diary;
using Xunit;

namespace Cpm4.UnitTests.FullMonthCalculatorTests
{
    public class GetForPosition
    {
        [Theory]

        [InlineData("2018-01-01", 1, "2018-01-01", 5, 1)]
        [InlineData("2018-01-03", 1, "2017-12-27", 6, 1)]
        [InlineData("2017-12-29", 1, "2017-12-29", 5, 1)]

        [InlineData("2018-01-01", 2, "2018-01-29", 5, 2)]
        public void Returns_correct(
            string yearStartString, 
            int requestedPosition, 
            string expectedFirstDay, 
            int expectedWeeks, 
            int expectedPosition
            )
        {
            var expected = new DiaryRange
            {
                FirstDay = Helpers.ToDate(expectedFirstDay),
                NumberOfWeeks = expectedWeeks,
                Position = expectedPosition,
            };
            var yearStart = Helpers.ToDate(yearStartString);
            var sut = new FullMonthCalculator();

            var actual = sut.GetForPosition(yearStart, requestedPosition);

            Assert.Equal(expected, actual);
        }
    }
}
