using Cpm.Core;
using Xunit;

namespace Cpm4.UnitTests.VirtualArrayTests
{
    public class Newly_created
    {
        public Newly_created()
        {
            Sut = new VirtualArray<string>();
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
