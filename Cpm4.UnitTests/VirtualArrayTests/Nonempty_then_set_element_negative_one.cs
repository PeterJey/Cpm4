using Cpm.Core;
using Xunit;

namespace Cpm4.UnitTests.VirtualArrayTests
{
    public class Nonempty_then_set_element_negative_one
    {
        public Nonempty_then_set_element_negative_one()
        {
            Sut = new VirtualArray<string>();
            Sut.Add("def");
            Sut.SetAt(-1, "abc");
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
        public void Offset_is_minus_one()
        {
            Assert.Equal(-1, Sut.Offset);
        }
    }
}
