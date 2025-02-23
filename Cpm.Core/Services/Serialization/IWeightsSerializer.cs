using System.Collections.Generic;

namespace Cpm.Core.Services.Serialization
{
    public interface IWeightsSerializer
    {
        string Serialize(IEnumerable<decimal?> values);
        IEnumerable<decimal?> Deserialize(string text);
    }
}