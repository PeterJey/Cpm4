using System.Collections.Generic;
using System.Linq;
using Cpm.Core.Models;
using Cpm.Core.Services.Notes;
using Cpm.Core.Services.Serialization;

namespace Cpm.Core.Services.Fields
{
    public class SiteDetails
    {
        public IReadOnlyCollection<FieldDetails> Fields { get; }

        public SiteDetails(
            Site site, 
            IYieldSerializer yieldSerializer, 
            IWeightsSerializer weightsSerializer,
            IPictureMetadataSerializer pictureSerializer
            )
        {
            Fields = site.Fields
                .OrderBy(x => x.Order)
                .Select(x => new FieldDetails(x, yieldSerializer, weightsSerializer, pictureSerializer))
                .ToArray();
        }
    }
}