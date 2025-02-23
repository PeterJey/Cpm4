using System.Collections.Generic;
using Cpm.Core;
using Xunit;

namespace Cpm4.UnitTests.VirtualArrayTests
{
    public class Created_from_nonempty_array
    {
        public Created_from_nonempty_array()
        {
            InputArray = new[]
            {
                "abc",
                "def",
                "ghi"
            };
         
            Sut = new VirtualArray<string>(InputArray);
        }

        public IEnumerable<string> InputArray { get; set; }

        public VirtualArray<string> Sut { get; }

        [Fact]
        public void Materializes_to_equal_array()
        {
            Assert.Equal(InputArray, Sut.ToArray());
        }

        [Fact]
        public void Offset_is_zero()
        {
            Assert.Equal(0, Sut.Offset);
        }
    }
}
