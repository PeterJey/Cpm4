using System.Threading.Tasks;
using Cpm.Core.Services.Profiles;

namespace Cpm.Core.Services.Forecast
{
    public interface ITemperatureProfileRepository
    {
        Task<TemperatureProfileResult> FindProfile(string location, SeasonsProfile seasonsProfile);
    }
}