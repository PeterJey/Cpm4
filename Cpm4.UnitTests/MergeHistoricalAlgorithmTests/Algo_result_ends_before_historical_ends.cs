//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Cpm.Core.Extensions;
//using Cpm.Core.Services;
//using Cpm.Core.Services.Forecast;
//using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
//using Xunit;

//namespace Cpm4.UnitTests.MergeHistoricalAlgorithmTests
//{
//    public class Algo_result_ends_before_historical_ends
//    {
//        public Algo_result_ends_before_historical_ends()
//        {
//            HistoricalStartingWeek = 21;
//            var forecastedValues = new decimal[] { 10, 20, 30 };
//            WeeklyHistoricWeights = new decimal[] {100, 200};
//            var sut = new MergingAlgorithm(new DummyAlgorithm
//            {
//                Output = new AlgorithmOutput
//                {
//                    Values = forecastedValues
//                            .Select(x => new HarvestValue
//                            {
//                                Weight = x,
//                                Type = HarvestValueType.Actual,
//                            }).ToList(),
//                    StartingWeek = HistoricalStartingWeek - 1,
//                },
//            });
//            var firstWeekCommencing = new DateTime(2018, 1, 1);
//            var input = new AlgorithmInput
//            {
//                History = new HarvestHistory(
//                    firstWeekCommencing.AddWeeks(HistoricalStartingWeek - 1), 
//                    firstWeekCommencing,
//                    WeeklyHistoricWeights
//                        .SelectMany(x => Enumerable.Repeat((decimal?)null, 6).Prepend(x))
//                        .ToArray()
//                    ),
//                Budget = new YieldPerHectare(0),
//            };

//            Result = sut.Calculate(input).Result;
//        }

//        public decimal[] WeeklyHistoricWeights { get; }

//        public AlgorithmOutput Result { get; }

//        public int HistoricalStartingWeek { get; }

//        [Fact]
//        public void StartingWeek_same_as_historical()
//        {
//            Assert.Equal(HistoricalStartingWeek, Result.StartingWeek);
//        }

//        [Fact]
//        public void Correct_values()
//        {
//            Assert.Equal(WeeklyHistoricWeights, Result.Values.Select(x => x.Weight));
//        }

//        [Fact]
//        public void Correct_value_types()
//        {
//            Assert.Equal(Enumerable.Repeat(HarvestValueType.Actual, 2), Result.Values.Select(x => x.Type));
//        }
//    }
//}
