using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cpm.Core.Services.Profiles
{
    public interface IProfileRepository
    {
        Task<MatchedProfile> FindMatch(string profileName, SeasonsProfile seasonsProfile);
        Task<IEnumerable<MatchedProfile>> GetByNamePattern(string pattern);
        Task<string> Save(string id, Func<MatchedProfile, string> updateFunc);
        Task Delete(string id);
    }
}