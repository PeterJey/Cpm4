using System.Threading;
using System.Threading.Tasks;
using Cpm.Core.Extensions;
using Cpm.Core.Services.Profiles;

namespace Cpm.Core.Services.Forecast
{
    public class OriginalAlgorithm : IAlgorithm
    {
        private readonly IProfileRepository _profileRepository;

        public OriginalAlgorithm(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        public async Task<AlgorithmOutput> Calculate(AlgorithmInput input, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.FindMatch(input.ProfileName, input.SeasonsProfile);

            if (profile == null)
            {
                return AlgorithmOutput.NoProfile;
            };

            var naiveTotal = input.Budget.KgPerHectare * input.AreaInHectares * (1 + profile.ExtraPotential);

            var weekOffset = int.Parse(input.Settings.GetOrDefault("WeekOffset", "0"));

            return AlgorithmOutput.FromProfile(profile, naiveTotal, weekOffset);
        }
    }
}