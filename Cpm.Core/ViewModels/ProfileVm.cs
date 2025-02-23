using System;
using System.Collections.Generic;
using System.Linq;
using Cpm.Core.Extensions;
using Cpm.Core.Services.Profiles;
using Cpm.Core.Services.Scenarios;

namespace Cpm.Core.ViewModels
{
    public class ProfileVm
    {
        public ProfileVm(IEnumerable<MatchedProfile> variants)
        {
            var local = variants
                //.OrderBy(x => x.Order)
                .OrderBy(x => x.SeasonsProfile, SeasonsProfile.DisplayComparer)
                .ToArray();

            if (local.Length == 0)
            {
                throw new ArgumentException("Empty collection, expected at least one profile variant");
            }

            Name = local[0].Name;
            var seasons = new HashSet<Season>();
            local
                .ForEach(x => seasons.UnionWith(x.Seasons));
            SeasonsSummary = string.Join(", ", seasons);
            Variants = local;
        }

        public IReadOnlyCollection<MatchedProfile> Variants { get; set; }

        public string SeasonsSummary { get; set; }

        public string Name { get; set; }
    }
}