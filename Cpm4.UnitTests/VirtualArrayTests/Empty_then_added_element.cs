using Cpm.Core;
using Xunit;

namespace Cpm4.UnitTests.VirtualArrayTests
{
    public class Empty_then_added_element
    {
        public Empty_then_added_element()
        {
            Sut = new VirtualArray<string>();

            Sut.Add("abc");
        }

        public VirtualArray<string> Sut { get; }

        [Fact]
        public void Materializes_to_single_element_array()
        {
            Assert.Equal(1, Sut.ToArray().Length);
        }

        [Fact]
        public void Zero_element_equals_added_value()
        {
            Assert.Equal("abc", Sut.ToArray()[0]);
        }

        [Fact]
        public void Offset_is_zero()
        {
            Assert.Equal(0, Sut.Offset);
        }
    }
}
