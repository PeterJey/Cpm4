using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cpm.Core;
using Cpm.Core.Services;
using Cpm.Core.Services.Diary;
using Cpm.Core.Services.Fields;
using Cpm.Core.Services.Notes;
using Cpm.Web.Helpers;
using Cpm.Web.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Optional.Unsafe;

namespace Cpm.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class DiaryController : Controller
    {
        private readonly IDiaryManager _diaryManager;
        private readonly IHarvestManager _harvestManager;
        private readonly IFieldRepository _fieldRepository;
        private readonly IPictureRepository _pictureRepository;
        private readonly HarvestPositionResolver _monthlyPositionResolver;
        private readonly FullMonthCalculator _monthCalculator;
        private readonly IDiaryRangeCalculator _weeklyCalculator;
        private readonly ICalendarPositionResolver _weeklyPositionResolver;

        public DiaryController(
            IDiaryManager diaryManager, 
            IHarvestManager harvestManager,
            IFieldRepository fieldRepository,
            IPictureRepository pictureRepository
            )
        {
            _diaryManager = diaryManager;
            _harvestManager = harvestManager;
            _fieldRepository = fieldRepository;
            _pictureRepository = pictureRepository;

            _monthCalculator = new FullMonthCalculator();

            _monthlyPositionResolver = new HarvestPositionResolver(
                _monthCalculator, 
                new NotesPositionResolver(
                    _monthCalculator,
                    new DateToPositionResolver(
                        _monthCalculator,
                        new IntegerPositionResolver(
                            new UseTodayResolver(
                                _monthCalculator
                                )
                            )
                        )
                    )
                );

            _weeklyCalculator = new OneWeekCalculator();

            _weeklyPositionResolver = new DateToPositionResolver(
                    _weeklyCalculator,
                    new IntegerPositionResolver(
                        new UseTodayResolver(
                            _weeklyCalculator
                        )
                    )
                );
        }

        [HttpGet]
        public async Task<IActionResult> Details(string fieldId = null, string day = null)
        {
            if (string.IsNullOrEmpty(day) || string.IsNullOrEmpty(fieldId))
            {
                return BadRequest();
            }

            if (!DateTime.TryParseExact(day, "yyyy-MM-dd", null, DateTimeStyles.None, out var date))
            {
                return BadRequest();
            }

            if (!User.CanViewDiaryForField(fieldId))
            {
                return Forbid();
            }

            var diaryDayVm = await _diaryManager.GetDayDetailsVm(fieldId, date);

            diaryDayVm.ShowChangeHarvest = User.CanChangeActualDataForField(fieldId)
                && date <= Clock.Now.Date;
            diaryDayVm.ShowPlanning = User.CanChangeDailyPlanForField(fieldId);
            diaryDayVm.ShowChangeDiary = User.CanChangeDiaryForField(fieldId);

            return PartialView("_DiaryDayDetails", diaryDayVm);
        }

        [HttpGet]
        public async Task<IActionResult> Calendar(string fieldId = null, string which = null)
        {
            if (string.IsNullOrEmpty(fieldId))
            {
                return BadRequest();
            }

            if (!User.CanViewDiaryForField(fieldId))
            {
                return Forbid();
            }

            var fieldDetails = await _fieldRepository.GetFieldById(fieldId);

            if (fieldDetails == null)
            {
                return NotFound();
            }

            var newPosition = _monthlyPositionResolver
                .Resolve(fieldDetails, which)
                .ValueOrFailure();

            
            var diaryRange = _monthCalculator.GetForPosition(fieldDetails.FirstWeekCommencing, newPosition);

            var diaryVm = await _diaryManager.GetDiaryVm(
                fieldId, 
                newPosition, 
                diaryRange.FirstDay, 
                diaryRange.NumberOfWeeks);

            return PartialView("_DiaryCalendar", diaryVm);
        }

        [HttpGet]
        public async Task<IActionResult> WeekOverview(string siteId, string which = null)
        {
            if (string.IsNullOrEmpty(siteId))
            {
                return BadRequest();
            }

            if (!User.CanViewDiaryForSite(siteId))
            {
                return Forbid();
            }

            var siteDetails = await _fieldRepository.GetSiteById(siteId);

            if (siteDetails == null)
            {
                return NotFound();
            }

            var firstField = siteDetails.Fields.First();

            var newPosition = _weeklyPositionResolver
                .Resolve(firstField, which)
                .ValueOrFailure();

            var diaryRange = _weeklyCalculator.GetForPosition(firstField.FirstWeekCommencing, newPosition);

            var viewModel = await _diaryManager.GetWeeklyOverviewVm(siteDetails, newPosition, diaryRange.FirstDay);

            return PartialView("_DiaryWeeklyOverview", viewModel);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> SaveHarvest(string fieldId, string day, decimal value, bool last)
        {
            if (string.IsNullOrEmpty(fieldId))
            {
                return BadRequest("fieldId");
            }

            if (!User.CanChangeActualDataForField(fieldId))
            {
                return Forbid();
            }

            if (!DateTime.TryParseExact(day, "yyyy-MM-dd", null, DateTimeStyles.None, out var date))
            {
                return BadRequest("day");
            }

            await _harvestManager.SetHarvestedWeight(fieldId, date, value, last);

            return Ok();
        }


        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> SavePickPlan(string fieldId, string day, decimal value)
        {
            if (string.IsNullOrEmpty(fieldId))
            {
                return BadRequest("fieldId");
            }

            if (!User.CanChangeDailyPlanForField(fieldId))
            {
                return Forbid();
            }

            if (!DateTime.TryParseExact(day, "yyyy-MM-dd", null, DateTimeStyles.None, out var date))
            {
                return BadRequest("day");
            }

            await _harvestManager.SetPlannedPick(fieldId, date, value);

            return Ok();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> SaveNote(string fieldId, string day, string note)
        {
            if (string.IsNullOrEmpty(fieldId))
            {
                return BadRequest("fieldId");
            }

            if (!User.CanChangeDiaryForField(fieldId))
            {
                return Forbid();
            }

            if (!DateTime.TryParseExact(day, "yyyy-MM-dd", null, DateTimeStyles.None, out var date))
            {
                return BadRequest("day");
            }

            await _fieldRepository.SaveFieldNote(fieldId, date, n =>
            {
                n.Text = note;
                if (n.IsDeleted)
                {
                    // as if created new
                    n.IsDeleted = false;
                    n.SerializedPictureMetadata = null;
                }
            });

            return Ok();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> DeleteNote(string fieldId, string day)
        {
            if (string.IsNullOrEmpty(fieldId))
            {
                return BadRequest("fieldId");
            }

            if (!User.CanChangeDiaryForField(fieldId))
            {
                return Forbid();
            }

            if (!DateTime.TryParseExact(day, "yyyy-MM-dd", null, DateTimeStyles.None, out var date))
            {
                return BadRequest("day");
            }

            await _fieldRepository.SaveFieldNote(fieldId, date, n => n.IsDeleted = true);

            return Ok();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> UploadPicture(string fieldId, string day, string tag, IFormFile file)
        {
            if (file == null)
            {
                return BadRequest("file");
            }

            if (string.IsNullOrEmpty(fieldId))
            {
                return BadRequest("fieldId");
            }

            if (!DateTime.TryParseExact(day, "yyyy-MM-dd", null, DateTimeStyles.None, out var date))
            {
                return BadRequest("day");
            }

            if (!User.CanChangeDiaryForField(fieldId))
            {
                return Forbid();
            }

            var field = await _fieldRepository.GetFieldById(fieldId);

            if (field == null)
            {
                throw new Exception($"Field with id {fieldId} not found");
            }

            await _pictureRepository.Store(field.SiteId, fieldId, date, tag, file.OpenReadStream(), file.FileName);
            return Ok();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> DeletePicture(string fieldId, string day, string id)
        {
            if (string.IsNullOrEmpty(fieldId))
            {
                return BadRequest("fieldId");
            }

            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("id");
            }

            if (!DateTime.TryParseExact(day, "yyyy-MM-dd", null, DateTimeStyles.None, out var date))
            {
                return BadRequest("day");
            }

            if (!User.CanChangeDiaryForField(fieldId))
            {
                return Forbid();
            }

            var field = await _fieldRepository.GetFieldById(fieldId);

            if (field == null)
            {
                throw new Exception($"Field with id {fieldId} not found");
            }

            await _pictureRepository.DeletePicture(fieldId, date, id);

            return Ok();
        }
    }
}