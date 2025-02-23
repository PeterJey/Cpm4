using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cpm.Core.Extensions;
using Cpm.Core.Services.Context;
using Cpm.Core.Services.Diary;
using Cpm.Core.Services.Fields;
using Cpm.Core.Services.Profiles;
using Cpm.Core.ViewModels;
using Microsoft.Extensions.Logging;
using Optional;

namespace Cpm.Core.Services.Forecast
{
    public class ForecastManager : IForecastManager
    {
        private readonly IUserPreferences _userPreferences;
        private readonly IFieldRepository _fieldRepository;
        private readonly IForecastService _forecastService;
        private readonly IForecastStatsServiceProvider _forecastStatsServiceProvider;
        private readonly ILogger<ForecastManager> _logger;

        public ForecastManager(
            IUserPreferences userPreferences,
            IFieldRepository fieldRepository,
            IForecastService forecastService,
            IForecastStatsServiceProvider forecastStatsServiceProvider,
            ILogger<ForecastManager> logger
        )
        {
            _userPreferences = userPreferences;
            _fieldRepository = fieldRepository;
            _forecastService = forecastService;
            _forecastStatsServiceProvider = forecastStatsServiceProvider;
            _logger = logger;
        }

        public async Task<ScenarioControlVm> GetScenarioControlVmForContext(ScenarioContext context)
        {
            var availableAlgorithms = _forecastService.GetAvailableAlgorithms()
                .Select(x => new OptionVm(x, x));

            var results = (await NewCalculationTaskForContext(context))
                .ToArray();

            return new ScenarioControlVm
            {
                ContextId = context.Id,
                Name = context.Name,
                SeasonScoresVm = CreateSeasonsScoreVm(context.SeasonsProfile, results),
                AreaUnit = _userPreferences.AreaUnit,
                Fields = results
                    .Select((x, index) => new ScenarioControlFieldInfo
                    {
                        FieldId = x.FieldDetails.FieldId,
                        Name = x.FieldDetails.FieldName,
                        IsVisible = context.FieldStates.ElementAt(index).IsVisible,
                        Variety = x.FieldDetails.Variety,
                        Area = _userPreferences.FormatArea(x.FieldDetails.AreaInHectares),
                        Algorithm = x.Result.AlgorithmName,
                        Algorithms = availableAlgorithms,
                        WeekOffset = context.FieldStates.ElementAt(index).Settings["WeekOffset"],
                        Quality = x.Result.Quality.ToString(),
                        Comments = x.Result.Comments,
                        AffectingSeasons = string.Join(
                            ",", 
                            x.Result. Seasons
                                .Select(s => s.ToString())
                        ),
                        Budget = _userPreferences.FormatYield(x.FieldDetails.Budget),
                        Target = _userPreferences.FormatYield(x.FieldDetails.Budget.ScaleBy(1 + x.Result.Difference)),
                        Relative = $"{(x.Result.Difference + 1) * 100:N0} %",
                        IsSignificantlyHigher = x.Result.Difference > 0.05m,
                        IsSignificantlyLower = x.Result.Difference < -0.05m,
                    })
                    .ToArray(),
            };
        }


        public async Task<GridResultsVm> GetResultsForContext(ScenarioContext context)
        {
            var visibleResults = (await NewCalculationTaskForContext(context))
                .Where(x => context.FieldStates.ElementAt(x.FieldDetails.Index).IsVisible)
                .ToArray();

            var subTotals = visibleResults
                .Select(x =>
                    x.Result.Weeks.Sum(v => v?.Value?.Weight ?? 0)
                )
                .ToArray();

            return new GridResultsVm
            {
                Series = new GridResultTopHeaderVm
                    {
                        Columns = visibleResults
                            .Select(x => x.FieldDetails.FieldName)
                            .ToList(),
                    },
                SummaryHeader = new GridResultSummaryHeaderVm
                    {
                        Subtotals = subTotals
                            .Select(x => x.ToString("N0"))
                            .ToList(),
                        GrandTotal = subTotals
                            .Sum()
                            .ToString("N0"),
                    },
                Rows = GridResultRowVms(context.ScenarioId, visibleResults)
                    .ToList(),
            };
        }

        public async Task<AlgorithmControlVm> GetAlgorithmControlVm(
            ScenarioContext context, 
            uint index, 
            uint extraWeeksBefore,
            uint extraWeeksAfter
            )
        {
            var results = (await NewCalculationTaskForAllAlgorithms(context, index))
                .ToArray();

            var fieldDetails = results
                .First()
                .FieldDetails;

            var selectedAlgorithm = context.FieldStates.ElementAt((int) index).Algorithm;
            var algoritmNames = results.Select(a => a.Result.AlgorithmName);

            return new AlgorithmControlVm
            {
                ContextId = context.Id,
                ScenarioName = context.Name,
                Index = index,
                FieldName = fieldDetails.FieldName,
                SeasonScoresVm = CreateSeasonsScoreVm(context.SeasonsProfile, results),
                SelectedAlgorithm = selectedAlgorithm,
                NamesRow = algoritmNames,
                TargetRow = results
                    .Select(a =>
                        _userPreferences.FormatYield(fieldDetails.Budget.ScaleBy(1 + a.Result.Difference))
                    ),
                RelativeYieldRow = results
                    .Select(a => new RelativeYieldVm
                    {
                        Relative = $"{(a.Result.Difference + 1) * 100:N0} %",
                        IsSignificantlyHigher = a.Result.Difference > 0.05m,
                        IsSignificantlyLower = a.Result.Difference < -0.05m,
                    }),
                Comments = results.Select(x => x.Result.Comments).ToArray(),
                Rows = GridResultRowVms(context.ScenarioId, results, extraWeeksBefore, extraWeeksAfter)
                    .ToList(),
            };
        }

        private static SeasonScoresVm CreateSeasonsScoreVm(SeasonsProfile profile, IEnumerable<ForecastOutput> results)
        {
            return new SeasonScoresVm(
                profile, 
                results
                    .SelectMany(x => x.Result.Seasons)
                    .Distinct()
                );
        }

        private async Task<IEnumerable<ForecastOutput>> NewCalculationTaskForContext(ScenarioContext context)
        {
            var site = await _fieldRepository.GetSiteById(context.SiteId);

            return await Calculate(site.Fields
                .Select((field, index) =>
                {
                    var fieldState = context.FieldStates.ElementAt(index);
                    return new ForecastInput
                    {
                        FieldDetails = field,
                        Parameters = new ForecastParameters(
                            context.SeasonsProfile,
                            fieldState.Algorithm,
                            fieldState.Settings
                        )
                    };
                }),
                context.IsDirty ? null : context.ScenarioId
                );
        }

        private async Task<IEnumerable<ForecastOutput>> NewCalculationTaskForAllAlgorithms(ScenarioContext context, uint index)
        {
            var site = await _fieldRepository.GetSiteById(context.SiteId);

            var field = site.Fields.ElementAt((int)index);

            return await Calculate(
                _forecastService.GetAvailableAlgorithms()
                    .Select(algo => new ForecastInput
                    {
                        FieldDetails = field,
                        Parameters = new ForecastParameters(
                            context.SeasonsProfile,
                            algo,
                            context.FieldStates.ElementAt((int)index).Settings
                        )
                    }),
                context.ScenarioId
            );
        }

        private async Task<IEnumerable<ForecastOutput>> Calculate(IEnumerable<ForecastInput> requests,
            string scenarioId)
        {
            using (var forecastLogger = _forecastStatsServiceProvider.GetInstance())
            {
                Task RecordFunc(string fl, ForecastResult rs) => string.IsNullOrEmpty(scenarioId)
                    ? Task.CompletedTask
                    : forecastLogger?.Update(scenarioId, fl, rs);

                var pool = requests
                    .Select(r => new
                    {
                        Request = r,
                        Task = _forecastService.Calculate(r.FieldDetails, r.Parameters, RecordFunc, CancellationToken.None)
                    })
                    .ToArray();

                await Task.WhenAll(pool.Select(x => x.Task));

                var result = pool
                    .Select(x => new ForecastOutput
                    {
                        FieldDetails = x.Request.FieldDetails,
                        Parameters = x.Request.Parameters,
                        Result = x.Task.Result,
                    })
                    .ToArray();

                await forecastLogger.Save();

                return result;
            }
        }

        private IEnumerable<GridResultRowVm> GridResultRowVms(
            string scenarioId,
            IReadOnlyCollection<ForecastOutput> results,
            uint extraWeeksBefore = 0,
            uint extraWeeksAfter = 0)
        {
            var firstWeekCommencing = results
                .FirstOrDefault()
                .SomeNotNull()
                .Map(x => x.FieldDetails.FirstWeekCommencing)
                .ValueOr(DateTime.MinValue);

            using (var forecastLogger = _forecastStatsServiceProvider.GetInstance())
            {
                var allStatsTasks = results
                    .Select(r =>
                        forecastLogger.GetStats(
                            scenarioId, 
                            r.FieldDetails.FieldId, 
                            r.Parameters.AlgorithmName
                            )
                        )
                    .ToArray();

                Task.WaitAll(allStatsTasks.Cast<Task>().ToArray());

                var allStats = allStatsTasks.Select(x => x.Result).ToArray();

                var aggregatedStats = allStats
                    .Where(x => x != null)
                    .Select(x => x.Weeks.Where(w => w != null).Select((w, i) => new {weekNumber =  x.StartingWeek + i, data = w}))
                    .Pivot(
                        s => s,
                        e => e.weekNumber,
                        (weekNumber, values, cols) =>
                        {
                            var localValues = cols.ToArray();
                            return new
                            {
                                WeekNumber = weekNumber,
                                MinWeight = localValues.Sum(x => x?.MinWeight),
                                MaxWeight = localValues.Sum(x => x?.MaxWeight),
                                MinManHours = localValues.Sum(x => x?.MinManHour),
                                MaxManHours = localValues.Sum(x => x?.MaxManHour),
                            };
                        },
                        (idx, week) => week?.data
                    ).ToArray();

                var showStats = true;

                return results
                    .Pivot(
                        s => s.Result.Weeks,
                        e => e.Week,
                        (weekNumber, values, cols) =>
                        {
                            var localValues = values.ToArray();

                            var stats = aggregatedStats
                                .SingleOrDefault(x => x.WeekNumber == weekNumber);

                            var nonNullValues = localValues.Where(x => x != null).ToArray();

                            showStats &= !nonNullValues.Any() || nonNullValues.All(x => x.Value.Type == HarvestValueType.Actual);

                            return new GridResultRowVm
                            {
                                Values = cols.ToArray(),
                                Week = weekNumber.ToString(),
                                Commencing = firstWeekCommencing
                                    .AddWeeks(weekNumber - 1)
                                    .ToString("d"),
                                Total = localValues
                                    .Sum(x => x?.Value?.Weight)
                                    .GetValueOrDefault(0)
                                    .ToString("N0"),
                                StatsWeightMin = stats?.MinWeight?.ToString("N0"),
                                StatsWeightMax = stats?.MaxWeight?.ToString("N0"),
                                StatsManHoursMin = stats?.MinManHours.ToOption().Map(x => x / 40).ToNullable()?.ToString("N0"),
                                StatsManHoursMax = stats?.MaxManHours.ToOption().Map(x => x / 40).ToNullable()?.ToString("N0"),
                                ShowStats = showStats,
                                Labour = localValues
                                    .Select(x => x?.Value?.ManHour)
                                    .NullableSum()
                                    .ToOption()
                                    .Map(x => (x / 40).ToString("N0"))
                                    .ValueOr(string.Empty),
                            };
                        },
                        (idx, week) =>
                        {
                            var v = week?.Value;
                            var stats = (allStats[idx] != null && week != null) 
                                ? allStats[idx]?.Weeks?.ElementAtOrDefault(week.Week - allStats[idx].StartingWeek) 
                                : null;
                            return new GridResultValueVm
                            {
                                Weight = v
                                    .SomeNotNull()
                                    .Map(x => x.Weight.ToString("N0"))
                                    .ValueOr(string.Empty),
                                IsActual = v?.Type == HarvestValueType.Actual,
                                IsInferred = v?.Type == HarvestValueType.Inferred,
                                StatsWeightMin = stats?.MinWeight.ToString("N0"),
                                StatsWeightMax = stats?.MaxWeight.ToString("N0"),
                                StatsManHoursMin = stats?.MinManHour.ToOption().Map(x => x / 40).ToNullable()?.ToString("N0"),
                                StatsManHoursMax = stats?.MaxManHour.ToOption().Map(x => x / 40).ToNullable()?.ToString("N0"),
                                ShowStats = v.SomeNotNull().Map(r => r.Type == HarvestValueType.Actual).ValueOr(false),
                            };
                        },
                        extraWeeksBefore,
                        extraWeeksAfter);
            }
        }
    }
}