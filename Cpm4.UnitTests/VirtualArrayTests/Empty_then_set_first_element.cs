using Cpm.Core;
using Xunit;

namespace Cpm4.UnitTests.VirtualArrayTests
{
    public class Empty_then_set_first_element
    {
        public Empty_then_set_first_element()
        {
            Sut = new VirtualArray<string>();

            Sut.SetAt(1, "abc");
        }

        public VirtualArray<string> Sut { get; }

        [Fact]
        public void Materializes_to_two_element_array()
        {
            Assert.Equal(2, Sut.ToArray().Length);
        }

        [Fact]
        public void Element_zero_equals_default_value()
        {
            Assert.Equal(null, Sut.ToArray()[0]);
        }

        [Fact]
        public void Element_one_equals_added_value()
        {
            Assert.Equal("abc", Sut.ToArray()[1]);
        }

        [Fact]
        public void Offset_is_zero()
        {
            Assert.Equal(0, Sut.Offset);
        }
    }
}
