using System.Linq;
using System.Threading.Tasks;
using Cpm.Core.Extensions;
using Cpm.Core.Services;
using Cpm.Core.Services.Context;
using Cpm.Core.Services.Forecast;
using Cpm.Core.Services.Profiles;
using Cpm.Core.Services.Scenarios;
using Cpm.FileExport;
using Cpm.Infrastructure.Data;
using Cpm.Web.Security;
using Microsoft.AspNetCore.Mvc;

namespace Cpm.Web.Controllers
{
    [Route("[controller]/[action]")]
    public class ScenariosController : Controller
    {
        private readonly IScenarioWorkspaceManager _workspaceManager;
        private readonly IForecastManager _forecastManager;
        private readonly IFieldManager _fieldManager;

        public ScenariosController(
            IScenarioWorkspaceManager workspaceManager,
            IForecastManager forecastManager,
            IFieldManager fieldManager
            )
        {
            _workspaceManager = workspaceManager;
            _forecastManager = forecastManager;
            _fieldManager = fieldManager;
        }

        // requested dirtectly from the link on a page, easier to use GET
        [HttpGet]
        public async Task<IActionResult> Load(string scenarioId = null)
        {
            if (string.IsNullOrEmpty(scenarioId))
            {
                return BadRequest();
            }

            var context = await _workspaceManager.LoadScenario(scenarioId);

            if (context == null)
            {
                return NotFound();
            }

            if (!User.CanChangeForecastForSite(context.SiteId))
            {
                await _workspaceManager.Discard(context.Id);
                return Forbid();
            }

            return RedirectToPage("/Scenarios/Control", new { contextid = context.Id });
        }
        
        // requested dirtectly from the link on a page, easier to use GET
        [HttpGet]
        public async Task<IActionResult> Create(string siteId = null)
        {
            if (string.IsNullOrEmpty(siteId))
            {
                return BadRequest();
            }

            if (!User.CanChangeForecastForSite(siteId))
            {
                return Forbid();
            }

            var context = await _workspaceManager.CreateScenario(siteId);

            if (context == null)
            {
                return NotFound();
            }

            return RedirectToPage("/Scenarios/Control", new { contextid = context.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActiveScenarioForFields(string contextId = null)
        {
            if (string.IsNullOrEmpty(contextId))
            {
                return BadRequest();
            }

            if (!_workspaceManager.TryGet(contextId, out var context))
            {
                return NotFound();
            }

            if (!User.CanChangeActiveScenarioForFieldsInSite(context.SiteId))
            {
                return Forbid();
            }

            var fieldIds = context
                .FieldStates
                .Zip(context.Fields, (state, fid) => state.IsVisible ? fid : null)
                .IgnoreNulls()
                .ToArray();

            foreach (var fieldId in fieldIds)
            {
                if (!await _fieldManager.UpdateActiveScenario(fieldId, context.ScenarioId))
                {
                    return NotFound();
                }
            }

            return Ok();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Rename(string contextId = null, string name = null)
        {
            if (string.IsNullOrEmpty(contextId) || string.IsNullOrEmpty(name))
            {
                return BadRequest();
            }

            if (!_workspaceManager.TryGet(contextId, out var context))
            {
                return NotFound();
            }

            if (!User.CanChangeForecastForSite(context.SiteId))
            {
                return Forbid();
            }

            _workspaceManager.Update(context.Rename(name));

            return Ok();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Save(string contextId = null)
        {
            if (string.IsNullOrEmpty(contextId))
            {
                return BadRequest();
            }

            if (!_workspaceManager.TryGet(contextId, out var context))
            {
                return NotFound();
            }

            if (!User.CanChangeForecastForSite(context.SiteId))
            {
                return Forbid();
            }

            if (!await _workspaceManager.Save(context.Id))
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Fields(string contextId = null)
        {
            if (string.IsNullOrEmpty(contextId))
            {
                return BadRequest();
            }

            if (!_workspaceManager.TryGet(contextId, out var context))
            {
                return NotFound();
            }

            if (!User.CanChangeForecastForSite(context.SiteId))
            {
                return Forbid();
            }

            var viewModel = await _forecastManager.GetScenarioControlVmForContext(context);

            return PartialView("_ScenarioControl", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ResultsGrid(string contextId)
        {
            if (string.IsNullOrEmpty(contextId))
            {
                return BadRequest();
            }

            if (!_workspaceManager.TryGet(contextId, out var context))
            {
                return NotFound();
            }

            if (!User.CanChangeForecastForSite(context.SiteId))
            {
                return Forbid();
            }

            var viewModel = await _forecastManager.GetResultsForContext(context);

            return PartialView("_ResultsGrid", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadGrid(string contextId)
        {
            if (string.IsNullOrEmpty(contextId))
            {
                return BadRequest();
            }

            if (!_workspaceManager.TryGet(contextId, out var context))
            {
                return NotFound();
            }

            if (!User.CanChangeForecastForSite(context.SiteId))
            {
                return Forbid();
            }

            var viewModel = await _forecastManager.GetResultsForContext(context);

            var result = await new ExcelWorksheetExporter().Export(viewModel);

            Response.Headers.Add("Content-Disposition", $"attachment;filename={result.FileName}");
            return File(result.Stream, result.ContentType);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult SelectedFields(string contextId = null, string selection = null)
        {
            if (string.IsNullOrEmpty(contextId) || string.IsNullOrEmpty(selection))
            {
                return BadRequest();
            }

            if (!_workspaceManager.TryGet(contextId, out var context))
            {
                return NotFound();
            }

            if (!User.CanChangeForecastForSite(context.SiteId))
            {
                return Forbid();
            }

            _workspaceManager.Update(
                    context.ChangeSelection(
                        selection
                            .ToLowerInvariant()
                            .ToCharArray()
                            .Select(c => c == '1')
                        )
                );

            return Ok();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult SelectedAlgorithm(string contextId, int index, string algorithm)
        {
            if (string.IsNullOrEmpty(contextId) || string.IsNullOrEmpty(algorithm))
            {
                return BadRequest();
            }

            if (!_workspaceManager.TryGet(contextId, out var context))
            {
                return NotFound();
            }

            if (!User.CanChangeForecastForSite(context.SiteId))
            {
                return Forbid();
            }

            _workspaceManager.Update(
                context.ChangeAlgorithm(index, algorithm)
            );

            return Ok();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult WeekOffset(string contextId, int index, int offset)
        {
            if (string.IsNullOrEmpty(contextId))
            {
                return BadRequest();
            }

            if (!_workspaceManager.TryGet(contextId, out var context))
            {
                return NotFound();
            }

            if (!User.CanChangeForecastForSite(context.SiteId))
            {
                return Forbid();
            }

            if (index < 0 || index >= context.FieldStates.Count)
            {
                return BadRequest();
            }

            if (offset < -5 || offset > 5)
            {
                return BadRequest();
            }

            _workspaceManager.Update(
                context.ChangeSettings(index, "WeekOffset", offset.ToString())
                );

            return Ok();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult SeasonScores(string contextId, string winter, string spring, string summer, string autumn)
        {
            if (string.IsNullOrEmpty(contextId))
            {
                return BadRequest();
            }

            if (!_workspaceManager.TryGet(contextId, out var context))
            {
                return NotFound();
            }

            if (!User.CanChangeForecastForSite(context.SiteId))
            {
                return Forbid();
            }

            _workspaceManager.Update(
                    context.ChangeSeasonScores(SeasonsProfile.FromScores(
                        new[]
                        {
                            new SeasonScore(Season.Winter, winter),
                            new SeasonScore(Season.Spring, spring),
                            new SeasonScore(Season.Summer, summer),
                            new SeasonScore(Season.Autumn, autumn),
                        })
                    )
                );

            return Ok();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(string contextId)
        {
            if (string.IsNullOrEmpty(contextId))
            {
                return BadRequest();
            }

            if (!_workspaceManager.TryGet(contextId, out var context))
            {
                return NotFound();
            }

            if (!User.CanChangeForecastForSite(context.SiteId))
            {
                return Forbid();
            }

            await _workspaceManager.Delete(context.Id);

            return Ok();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Duplicate(string contextId)
        {
            if (string.IsNullOrEmpty(contextId))
            {
                return BadRequest();
            }

            if (!_workspaceManager.TryGet(contextId, out var context))
            {
                return NotFound();
            }

            if (!User.CanChangeForecastForSite(context.SiteId))
            {
                return Forbid();
            }

            var newContext = await _workspaceManager.Duplicate(context.Id);

            if (newContext == null)
            {
                return BadRequest();
            }

            return Ok(new { contextid = newContext.Id });
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Settings(string contextId, int index, string algorithm, int week, string adjustedValues)
        {
            if (string.IsNullOrEmpty(contextId))
            {
                return BadRequest();
            }

            if (!_workspaceManager.TryGet(contextId, out var context))
            {
                return NotFound();
            }

            if (!User.CanChangeForecastForSite(context.SiteId))
            {
                return Forbid();
            }

            if (index < 0 || index >= context.FieldStates.Count)
            {
                return BadRequest();
            }

            if (week < 0 || week > 52)
            {
                return BadRequest();
            }

            if (string.IsNullOrEmpty(algorithm))
            {
                return BadRequest();
            }

            var values = adjustedValues
                .Split(",")
                .Select(x => string.IsNullOrEmpty(x) ? (decimal?)null : decimal.Parse(x))
                .SkipNullsFromEnd()
                .ToList();

            var trailingNulls = values.Count - values.SkipNulls().Count();

            var weekOffset = int.Parse(context.FieldStates.ElementAt(index).Settings["WeekOffset"]);

            _workspaceManager.Update(
                    context
                        .ChangeAlgorithm(index, algorithm)
                        .ChangeSettings(
                            index, 
                            "AdjustedStartingWeek", 
                            (week + trailingNulls - weekOffset).ToString()
                            )
                        .ChangeSettings(
                            index, 
                            "AdjustedValues",
                            string.Join(
                                ",",
                                values
                                    .SkipFromEnd(x => x.GetValueOrDefault(0) == 0)
                                    .Select(x => x.GetValueOrDefault(0))
                            )
                        )
                );

            return Ok();
        }
    }
}