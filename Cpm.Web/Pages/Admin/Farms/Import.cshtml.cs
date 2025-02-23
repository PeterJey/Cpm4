using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cpm.Core.Farms;
using Cpm.Web.PageHelpers;
using Cpm.Web.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cpm.Web.Pages.Admin.Farms
{
    public class ImportModel : StatusAwarePageModel
    {
        private readonly IFarmManager _farmManager;

        public ImportModel(IFarmManager farmManager)
        {
            _farmManager = farmManager;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.CanManageFarms())
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Input.File.Length > 1024*200 /*200kiB*/)
            {
                SaveStatus()
                    .Warning()
                    .Text($"The file is over 200kB, please use a smaller file.")
                    .Dismissible();

                return Page();
            }

            if (Input.File.Length == 0)
            {
                SaveStatus()
                    .Warning()
                    .Text($"The file is empty.")
                    .Dismissible();

                return Page();
            }

            IModelResult modelResult;
            using (var reader = new StreamReader(Input.File.OpenReadStream()))
            {
                modelResult = FarmModel.Parse(await reader.ReadToEndAsync());
            }

            var farms = await _farmManager.GetAllFarms();

            if (farms.Any(x => x.Name.Equals(modelResult.Model.Name, StringComparison.InvariantCultureIgnoreCase)))
            {
                modelResult.Errors.Add("Farm with this name already exists.");
            }

            if (!modelResult.Success)
            {
                foreach (var error in modelResult.Errors)
                {
                    ModelState.AddModelError("", error);
                }

                return Page();
            }

            try
            {
                await _farmManager.CreateNewFarm(modelResult.Model);
            }
            catch (Exception ex)
            {
                SaveStatus()
                    .Danger()
                    .Text($"Failed creating the farm: {ex.Message}")
                    .Dismissible();
            }

            SaveStatus()
                .Success()
                .Text($"File \"{Input.File.Name}\" was processed successfully and the farm ")
                .Strong(modelResult.Model.Name)
                .Text(" was created.")
                .Dismissible();

            return RedirectToPage("./Index");
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string  Foo { get; set; }

            [Required]
            [DisplayName("Choose a JSON file with farm definition:")]
            public IFormFile File { get; set; }
        }
    }
}