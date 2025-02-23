using System.Linq;
using System.Threading.Tasks;
using Cpm.Core.Services.Forecast;
using Cpm.Core.Services.Profiles;
using Cpm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Cpm.Infrastructure
{
    public class EfTemperatureProfileRepository : ITemperatureProfileRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public EfTemperatureProfileRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TemperatureProfileResult> FindProfile(string location, SeasonsProfile seasonsProfile)
        {
            var profiles = await _dbContext.TempProfiles
                .AsNoTracking()
                .Where(x => x.Location == location)
                .ToListAsync();

            var match = profiles.FirstOrDefault(x => 
                JsonCriteriaSerializer.Instance
                    .Deserialize(x.SerializedCriteria)
                    .Includes(seasonsProfile)
                );

            if (match == null)
            {
                return TemperatureProfileResult.NotFound;
            }

            return TemperatureProfileResult.FromProfile(JsonConvert.DeserializeObject<decimal[]>(match.SerializedPoints));
        }
    }
}