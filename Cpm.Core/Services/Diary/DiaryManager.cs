using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cpm.Core.Extensions;
using Cpm.Core.Services.Fields;
using Cpm.Core.Services.Forecast;
using Cpm.Core.Services.Notes;
using Cpm.Core.Services.Scenarios;
using Cpm.Core.ViewModels;
using Optional;

namespace Cpm.Core.Services.Diary
{
    public class DiaryManager : IDiaryManager
    {
        private readonly IScenarioRepository _scenarioRepository;
        private readonly IForecastService _forecastService;
        private readonly IPictureRepository _pictureRepository;
        private readonly IFieldRepository _fieldRepository;
        private readonly IForecastStatsServiceProvider _forecastStatsServiceProvider;

        public DiaryManager(
            IScenarioRepository scenarioRepository,
            IForecastService forecastService,
            IPictureRepository pictureRepository,
            IFieldRepository fieldRepository,
            IForecastStatsServiceProvider forecastStatsServiceProvider
            )
        {
            _scenarioRepository = scenarioRepository;
            _forecastService = forecastService;
            _pictureRepository = pictureRepository;
            _fieldRepository = fieldRepository;
            _forecastStatsServiceProvider = forecastStatsServiceProvider;
        }

        private static IEnumerable<string> GetWeekdayNames(DateTime firstDay)
        {
            return Enumerable.Range(0, 7)
                .Select(x => firstDay.AddDays(x).ToString("ddd"));
        }

        public async Task<DiaryVm> GetDiaryVm(string fieldId, int newPosition, DateTime firstDay, int weeks)
        {
            var fieldDetails = await _fieldRepository.GetFieldById(fieldId);

            var firstWeek = firstDay.WeekNumber(fieldDetails.FirstWeekCommencing);

            var scenario = await _scenarioRepository.GetScenarioState(fieldDetails.ActiveScenarioId);
            var forecastParameters = scenario?.Parameters?.ElementAt(fieldDetails.Index);
            var forecast = await scenario
                .SomeNotNull()
                .Map(async s => await _forecastService.Calculate(
                    fieldDetails,
                    forecastParameters,
                    null,
                    CancellationToken.None
                ))
                .ValueOr(Task.FromResult<ForecastResult>(null));

            using (var forecastLogger = _forecastStatsServiceProvider.GetInstance())
            {
                var diary = new DiaryVm
                {
                    Weekdays = GetWeekdayNames(firstDay)
                        .ToList(),

                    Weeks = Enumerable.Range(firstWeek, weeks)
                        .Select(weekNumber => CreateDiaryWeek(
                            fieldDetails,
                            weekNumber,
                            forecast,
                            () => GetStats(forecastLogger, fieldDetails, forecastParameters?.AlgorithmName, weekNumber),
                            date => weekNumber == firstWeek && date.Day > 6 ||
                                    (weekNumber == firstWeek + weeks - 1) && date.Day < 22)
                        )
                        .ToArray(),

                    Position = newPosition,
                    Title = fieldDetails.FirstWeekCommencing
                        .AddDays(7) //ensure not in previous month
                        .AddMonths(newPosition - 1)
                        .ToString("MMMM yyyy"),
                    SiteId = fieldDetails.SiteId,
                };

                return diary;
            }
        }

        private static WeeklyStats GetStats(IForecastStatsService forecastLogger, FieldDetails fieldDetails, string algorithmName, int weekNumber)
        {
            var allStats = forecastLogger?.GetStats(fieldDetails.ActiveScenarioId, fieldDetails.FieldId, algorithmName)?.Result;

            if (allStats == null || allStats.Weeks.Length == 0) return null;

            return allStats.Weeks.ElementAtOrDefault(weekNumber - allStats.StartingWeek);
        }

        private DiaryWeekVm CreateDiaryWeek(FieldDetails fieldDetails, int weekNumber, ForecastResult forecast,
            Func<WeeklyStats> statistics, Func<DateTime, bool> getIsPadding)
        {
            var forecastValue = forecast?.Weeks
                ?.SingleOrDefault(f => f.Week == weekNumber)
                ?.Value;

            var firstDay = fieldDetails.FirstWeekCommencing
                .AddWeeks(weekNumber - 1);

            var stats = statistics?.Invoke();

            return new DiaryWeekVm
            {
                Days = CreateDiaryDays(
                        fieldDetails,
                        firstDay,
                        getIsPadding
                            )
                    .ToArray(),

                WeekNumber = weekNumber,

                ForecastedWeight = forecastValue?.Weight,

                IsForecastActualValue = forecastValue?.Type == HarvestValueType.Actual,

                IsForecastInferred = forecastValue?.Type == HarvestValueType.Inferred,
                
                FieldId = fieldDetails.FieldId,

                FieldName = fieldDetails.FieldName,

                IsCompleted = fieldDetails.HarvestHistory.Weekly.SingleOrDefault(x => x.WeekNumber == weekNumber)?.IsCompleted ?? false,

                StatsWeightMin = stats?.MinWeight.ToString("N0"),
                StatsWeightMax = stats?.MaxWeight.ToString("N0"),
                StatsManHoursMin = stats?.MinManHour.ToOption().Map(x => x / 40).ToNullable()?.ToString("N0"),
                StatsManHoursMax = stats?.MaxManHour.ToOption().Map(x => x / 40).ToNullable()?.ToString("N0"),

                ShowStats = forecastValue.SomeNotNull().Map(r => r.Type == HarvestValueType.Actual).ValueOr(false),
            };
        }

        private IEnumerable<DiaryDayVm> CreateDiaryDays(FieldDetails fieldDetails, DateTime firstDay, Func<DateTime, bool> getIsPadding)
        {
            return Enumerable.Range(0, 7)
                .Select(dayIndex => firstDay.AddDays(dayIndex))
                .Select(date => CreateDiaryDay(fieldDetails, date, getIsPadding))
                .ToArray();
        }

        private DiaryDayVm CreateDiaryDay(
            FieldDetails fieldDetails,
            DateTime date
        ) => CreateDiaryDay(fieldDetails, date, _ => false);

        private DiaryDayVm CreateDiaryDay(FieldDetails fieldDetails, DateTime date, Func<DateTime, bool> getIsPadding)
        {
            var weight = fieldDetails.HarvestHistory.Days
                .SingleOrDefault(x => x.Date == date)
                ?.Weight;
            var note = fieldDetails.Notes.SingleOrDefault(x => x.Date == date);

            return new DiaryDayVm
            {
                Date = date,
                Weight = weight,
                Planned =
                    fieldDetails.PickingPlan.Days.SingleOrDefault(x => x.Date == date)?.Weight,
                IsPadding = getIsPadding(date),
                Note = note,
                Pictures = note?.Pictures
                    ?.Select(x => new PictureVm
                    {
                        FullUrl = _pictureRepository.GetFullUrl(
                            fieldDetails.SiteId, 
                            fieldDetails.FieldId, 
                            date,
                            x.Id,
                            x.OriginalExtension),
                        ThumbUrl = _pictureRepository.GetThumbUrl(
                            fieldDetails.SiteId, 
                            fieldDetails.FieldId, 
                            date,
                            x.Id),
                        UploadedBy = x.UploadedBy,
                        TakenOn = x.TakenOn,
                        Id = x.Id,
                        HasLocation = x.Latitude.HasValue && x.Longitude.HasValue,
                        LocationLat = x.Latitude.GetValueOrDefault(0),
                        LocationLon = x.Longitude.GetValueOrDefault(0),
                    })?.ToArray() ?? new PictureVm[0],
                FieldName = fieldDetails.FieldName,
                CanBecomeLastHarvestOfTheWeek = fieldDetails.HarvestHistory.CanBecomeLastHarvestDay(date),
                CanBePlanned = fieldDetails.HarvestHistory.LastDay
                    .Map(ld => ld < date)
                    .ValueOr(true),
                FieldId = fieldDetails.FieldId,
                SiteId = fieldDetails.SiteId
            };
        }

        public async Task<WeeklyOverviewVm> GetWeeklyOverviewVm(SiteDetails site, int newPosition, DateTime firstDay)
        {
            using (var forecastLogger = _forecastStatsServiceProvider.GetInstance())
            {
                var fieldTasks = site.Fields
                    .Select(async field =>
                    {
                        var scenario = await _scenarioRepository.GetScenarioState(field.ActiveScenarioId);

                        var forecastParameters = scenario?.Parameters?.ElementAt(field.Index);

                        var result = await _forecastService.Calculate(
                            field, 
                            forecastParameters, 
                            (fl, rs) => forecastLogger?.Update(field.ActiveScenarioId, fl, rs),
                            CancellationToken.None
                            );

                        var week1 = CreateDiaryWeek(
                            field,
                            newPosition,
                            result,
                            () => GetStats(forecastLogger, field, forecastParameters?.AlgorithmName, newPosition),
                            _ => false
                        );

                        var week2 = CreateDiaryWeek(
                            field,
                            newPosition + 1,
                            result,
                            () => GetStats(forecastLogger, field, forecastParameters?.AlgorithmName, newPosition+1),
                            _ => false
                        );

                        return new DiaryWeekFieldVm
                        {
                            Week1 = week1,
                            Week2 = week2,
                        };
                    })
                    .ToArray();

                await Task.WhenAll(fieldTasks);

                await forecastLogger.Save();

                var fields = fieldTasks.Select(x => x.Result).ToArray();

                return new WeeklyOverviewVm
                {
                    Position = newPosition,
                    Title = $"Week {newPosition} & {newPosition + 1}",
                    DayHeaderWeek1 = CreateDayHeader(firstDay),
                    DayHeaderWeek2 = CreateDayHeader(firstDay.AddDays(7)),
                    Fields = fields,
                    Week1Totals = fields.SelectMany(x => x.Week1.Days).GroupBy(day => day.Date).OrderBy(gr => gr.Key).Select(gr => gr.Sum(day => day.Weight ?? 0)).ToArray(),
                    Week2Totals = fields.SelectMany(x => x.Week2.Days).GroupBy(day => day.Date).OrderBy(gr => gr.Key).Select(gr => gr.Sum(day => day.Weight ?? 0)).ToArray(),
                };
            }
        }

        private static List<DayHeaderItem> CreateDayHeader(DateTime firstDay)
        {
            return Enumerable.Range(0, 7)
                .Select(i => firstDay.AddDays(i))
                .Select(d => new DayHeaderItem
                {
                    Day = d.Day,
                    Month = d.ToString("MMM"),
                    Weekday = d.ToString("ddd"),
                    IsToday = d == Clock.Now.Date
                })
                .ToList();
        }

        public async Task<DiaryDayVm> GetDayDetailsVm(string fieldId, DateTime date)
        {
            var fieldDetails = await _fieldRepository.GetFieldById(fieldId);

            return CreateDiaryDay(fieldDetails, date);
        }
    }
}