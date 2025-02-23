using System.Linq;
using Cpm.Core.Extensions;
using Xunit;

namespace Cpm4.UnitTests.EnumerableExtensionsTests
{
    public class Tests
    {
        [Fact]
        public void Short_delayed_inclusive()
        {
            var s1 = new[] { "1", "2", "3", "4" };
            var s2 = new[] { "a", "b" };
            var offset = 1;
            var expected = new[] { "2a", "3b" };

            var actual = s1.ZipWithOffset(s2, offset, Merge);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Two_same_length_zero_offset_are_zipped()
        {
            var s1 = new[] { "1", "2", "3" };
            var s2 = new[] { "a", "b", "c" };
            var expected = new[] { "1a", "2b", "3c" };

            var actual = s1.ZipWithOffset(s2, 0, Merge);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Two_empty_zero_offset_empty()
        {
            var s1 = Enumerable.Empty<string>();
            var s2 = Enumerable.Empty<string>();

            var actual = s1.ZipWithOffset(s2, 0, (a, b) => 0);

            Assert.Empty(actual);
        }

        private string Merge(string a, string b)
        {
            return a + b;
        }
    }
}
