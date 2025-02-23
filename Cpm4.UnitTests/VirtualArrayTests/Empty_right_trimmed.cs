using Cpm.Core;
using Xunit;

namespace Cpm4.UnitTests.VirtualArrayTests
{
    public class Empty_right_trimmed
    {
        public Empty_right_trimmed()
        {
            Sut = new VirtualArray<string>();

            Sut.RightTrim(x => x.Length < 3);
        }

        public VirtualArray<string> Sut { get; }

        [Fact]
        public void Materializes_to_empty_array()
        {
            Assert.Empty(Sut.ToArray());
        }

        [Fact]
        public void Offset_is_zero()
        {
            Assert.Equal(0, Sut.Offset);
        }
    }
}
