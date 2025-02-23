using Cpm.Core;
using Xunit;

namespace Cpm4.UnitTests.VirtualArrayTests
{
    public class Empty_then_set_element_negative_one
    {
        public Empty_then_set_element_negative_one()
        {
            Sut = new VirtualArray<string>();

            Sut.SetAt(-1, "abc");
        }

        public VirtualArray<string> Sut { get; }

        [Fact]
        public void Materializes_to_one_element_array()
        {
            Assert.Equal(1, Sut.ToArray().Length);
        }

        [Fact]
        public void Element_zero_equals_added_value()
        {
            Assert.Equal("abc", Sut.ToArray()[0]);
        }

        [Fact]
        public void Offset_is_minus_one()
        {
            Assert.Equal(-1, Sut.Offset);
        }
    }
}
