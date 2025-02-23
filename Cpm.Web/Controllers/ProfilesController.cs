using System.Linq;
using System.Threading.Tasks;
using Cpm.Core.Services.Profiles;
using Cpm.Core.Services.Scenarios;
using Microsoft.AspNetCore.Mvc;

namespace Cpm.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class ProfilesController : Controller
    {
        private readonly IProfileRepository _profileRepository;

        public ProfilesController(
            IProfileRepository profileRepository
            )
        {
            _profileRepository = profileRepository;
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            await _profileRepository.Delete(id);

            return Ok();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Save(ProfileModel model)
        {
            if (model?.Weights?.Length != model?.Productivity?.Length)
            {
                ModelState.AddModelError(string.Empty, "Invalid weight and productivity format");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var seasonsProfile = SeasonsProfile.FromScores(new SeasonScore(model.Season, model.SeasonType));

            var error = await _profileRepository.Save(
                model.Id,
                profile =>
                {
                    if (!string.IsNullOrEmpty(profile.Id) && string.IsNullOrEmpty(model.Id))
                    {
                        return "This profile variant already exists";
                    }

                    profile.Name = model.ProfileName;
                    profile.StartingWeek = model.StartingWeek;
                    profile.SeasonsProfile = seasonsProfile;
                    profile.ExtraPotential = model.ExtraPotential;
                    profile.Quality = model.Quality;
                    profile.Points = model.Weights
                        .Zip(
                            model.Productivity, 
                            (w, p) => new ProfilePoint
                            {
                                Weight = w,
                                PerHour = p
                            })
                        .ToArray();
                    profile.Comments = model.Comments?.Split("\n");
                    profile.Description = model.Description;
                    return string.Empty; 
                });

            if (!string.IsNullOrEmpty(error))
            {
                return BadRequest(error);
            }

            return Ok();
        }
    }
}