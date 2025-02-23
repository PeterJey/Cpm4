using System.Threading.Tasks;
using Cpm.Core.Services;
using Cpm.Core.Services.Fields;
using Cpm.Core.ViewModels;
using Cpm.Web.PageHelpers;
using Cpm.Web.Security;
using Microsoft.AspNetCore.Mvc;

namespace Cpm.Web.Pages.Fields
{
    public class MonthlyDiaryModel : StatusAwarePageModel
    {
        private readonly IUserPreferences _userPreferences;
        private readonly IFieldRepository _fieldRepository;

        public MonthlyDiaryModel(
            IUserPreferences userPreferences,
            IFieldRepository fieldRepository
            )
        {
            _userPreferences = userPreferences;
            _fieldRepository = fieldRepository;
        }

        public async Task<IActionResult> OnGetAsync(string fieldId)
        {
            if (!User.CanViewDiaryForField(fieldId))
            {
                return Forbid();
            }

            var field = await _fieldRepository.GetFieldById(fieldId);

            if (field == null)
            {
                return NotFound();
            }

            YieldVm = new YieldVm(field.Budget, _userPreferences);
            AreaUnitCode = _userPreferences.AreaUnit;
            AreaUnitName = _userPreferences.AreaUnitName;

            FieldName = field.FieldName;
            FarmName = field.FarmName;
            SiteName = field.SiteName;
            SiteId = field.SiteId;

            FieldId = fieldId;

            ShowNotes = User.CanChangeDiaryForField(fieldId);

            return Page();
        }

        public string FieldId { get; set; }

        public string SiteId { get; set; }

        public string SiteName { get; set; }

        public string FarmName { get; set; }

        public string FieldName { get; set; }

        public string AreaUnitName { get; set; }

        public string AreaUnitCode { get; set; }

        public YieldVm YieldVm { get; set; }

        public bool ShowNotes { get; set; }
    }
}