using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cpm.Core.Extensions;
using Cpm.Core.Services.Fields;
using Cpm.Core.ViewModels;

namespace Cpm.Core.Services.Allocations
{
    public class AllocationManager : IAllocationManager
    {
        private readonly IAllocationRepository _allocationRepository;
        private readonly IUserPreferencesFactory _userPreferencesFactory;
        private readonly IFieldRepository _fieldRepository;

        public AllocationManager(
            IAllocationRepository allocationRepository,
            IUserPreferencesFactory userPreferencesFactory,
            IFieldRepository fieldRepository
            )
        {
            _allocationRepository = allocationRepository;
            _userPreferencesFactory = userPreferencesFactory;
            _fieldRepository = fieldRepository;
        }

        public async Task<AllocationVm> GetForSite(string siteId)
        {
            var site = await _fieldRepository.GetSiteById(siteId);

            var field = site?.Fields?.FirstOrDefault();

            return new AllocationVm
            {
                SiteId = field?.SiteId,
                SiteName = field?.SiteName,
                FarmName = field?.FarmName
            };
        }

        public async Task<AllocationWeekVm> GetWeekVm(string siteId, int? week)
        {
            var site = await _fieldRepository.GetSiteById(siteId);

            if (site == null)
            {
                throw new Exception($"Site {siteId} not found");
            }

            var field = site?.Fields?.FirstOrDefault();

            var firstWeekCommencing = field.FirstWeekCommencing;

            var weekNumber = week.GetValueOrDefault(Clock.Now.Date.WeekNumber(firstWeekCommencing));

            var firstDay = firstWeekCommencing.AddWeeks(weekNumber - 1);

            var allocations = await _allocationRepository.GetForSiteAndDates(siteId, firstDay, firstDay.AddDays(6));

            var availableForAllFields = Enumerable.Range(0, 7)
                .Select(x => firstDay.AddDays(x))
                .Select(date => new AvailabilityDay
                {
                    Date = date,
                    ByField = site.Fields
                        .Select(f => new
                        {
                            Field = f,
                            Actual = f.HarvestHistory.Days
                                .SingleOrDefault(x => x.Date == date)
                                ?.Weight,
                            Planned = f.PickingPlan.Days
                                .SingleOrDefault(x => x.Date == date)
                                ?.Weight
                        })
                        .Where(x => x.Actual.HasValue || x.Planned.HasValue)
                        .ToDictionary(
                            k => k.Field,
                            v => v.Actual ?? v.Planned
                        )
                })
                .ToArray();

            var days = availableForAllFields
                .GroupJoin(
                    allocations.Allocations,
                    av => av.Date,
                    al => al.Date,
                    (av, als) => CreateNewDay(av, als, allocations.UsedProducts))
                .ToArray();

            return new AllocationWeekVm
            {
                SiteId = siteId,
                Week = weekNumber,
                UsedProducts = allocations.UsedProducts.Select(x => x.ToDescription())
                    .ToArray(),
                AllProducts = allocations.AllProducts
                    .Select(x => new OptionVm(x.ToKey(), x.ToDescription())),
                Days = days,
                Summary = CreateNewDay(
                    new AvailabilityDay
                    {
                        ByField = availableForAllFields
                            .SelectMany(x => x.ByField)
                            .GroupBy(x => x.Key)
                            .ToDictionary(
                                k => k.Key,
                                v => v.Select(x => x.Value).NullableSum()
                                )
                    },
                    allocations.Allocations, 
                    allocations.UsedProducts
                    ),
                AvailableUnits = _userPreferencesFactory.AvailableAllocationUnits()
            };
        }

        private NewDayVm CreateNewDay(
            AvailabilityDay availability,
            IEnumerable<AllocationState> allocations,
            ICollection<Product> usedProducts
        )
        {
            var localAllocations = allocations.ToArray();

            var usedFields = localAllocations.Select(x => FieldVm.FromField(x.Field))
                .Concat(availability.ByField.Select(x => FieldVm.FromFieldDetails(x.Key)))
                .Distinct(new GenericEqualityComparer<FieldVm>(x => x.Index))
                .OrderBy(x => x.Index)
                .ToArray();

            var fields = usedFields
                .GroupJoin(
                    availability.ByField,
                    f => f.Index,
                    av => av.Key.Index,
                    (f, av) => new {Field = f, Available = av?.SingleOrDefault().Value}
                )
                .GroupJoin(
                    localAllocations,
                    av => av.Field.Index,
                    al => al.Field.Order,
                    (av, al) => new
                    {
                        Field = av.Field,
                        Available = av.Available,
                        Allocations = al
                    }
                )
                .Select(x => new FieldAllocationVm
                {
                    Field = x.Field,
                    ToAllocate = x.Available,
                    Total = x.Allocations
                        .Select(a => (decimal?) a.Weight)
                        .NullableSum(),
                    Products = usedProducts
                        .GroupJoin(
                            x.Allocations,
                            p => p,
                            al => al.Product,
                            (p, al) => ProductAllocationVm
                                .Create(
                                    p, 
                                    al.Select(a => (decimal?)a.Weight)
                                        .NullableSum()
                                    )
                            )
                        .ToArray()
                })
                .ToArray();
            return new NewDayVm
            {
                Date = availability.Date,
                Summary = new FieldAllocationVm
                {
                    Field = FieldVm.Placeholder,
                    ToAllocate = fields
                        .Select(x => x.ToAllocate)
                        .NullableSum(),
                    Total = fields
                        .Select(x => x.Total)
                        .NullableSum(),
                    Products = usedProducts
                        .Select(product => ProductAllocationVm
                            .Create(product, fields)
                        ).ToArray(),
                },
                Fields = fields
            };
        }
    }
}