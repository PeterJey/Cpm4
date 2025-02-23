using Cpm.Core;
using Xunit;

namespace Cpm4.UnitTests.VirtualArrayTests
{
    public class Nonempty_with_offset_negative_one_then_set_element_negative_two
    {
        public Nonempty_with_offset_negative_one_then_set_element_negative_two()
        {
            Sut = new VirtualArray<string>();
            Sut.SetAt(-1, "def");
            Sut.SetAt(-2, "abc");
        }

        public VirtualArray<string> Sut { get; }

        [Fact]
        public void Materializes_to_two_element_array()
        {
            Assert.Equal(2, Sut.ToArray().Length);
        }

        [Fact]
        public void Element_zero_equals_added_value()
        {
            Assert.Equal("abc", Sut.ToArray()[0]);
        }

        [Fact]
        public void Element_one_equals_existing_value()
        {
            Assert.Equal("def", Sut.ToArray()[1]);
        }

        [Fact]
        public void Offset_is_minus_two()
        {
            Assert.Equal(-2, Sut.Offset);
        }
    }
}
