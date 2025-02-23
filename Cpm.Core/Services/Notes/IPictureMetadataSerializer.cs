using System.Collections.Generic;

namespace Cpm.Core.Services.Notes
{
    public interface IPictureMetadataSerializer
    {
        string Serialize(IEnumerable<PictureMetadata> pictures);
        List<PictureMetadata> Deserialize(string text);
    }
}