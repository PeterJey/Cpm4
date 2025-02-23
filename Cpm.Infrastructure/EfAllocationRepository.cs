using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cpm.Core.Extensions;
using Cpm.Core.Services;
using Cpm.Core.Services.Allocations;
using Cpm.Infrastructure.Data;
using Cpm.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Cpm.Infrastructure
{
    public class EfAllocationRepository : IAllocationRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IAuditDataProvider _auditDataProvider;

        public EfAllocationRepository(
            ApplicationDbContext dbContext,
            IAuditDataProvider auditDataProvider
            )
        {
            _dbContext = dbContext;
            _auditDataProvider = auditDataProvider;
        }

        public async Task<DailyAllocations> GetForSiteAndDates(string siteId, DateTime firstDay, DateTime lastDay)
        {
            var site = await _dbContext.Sites
                .Include(x => x.Farm)
                .Include(x => x.Fields)
                .ThenInclude(x => x.Allocations)
                .Include(x => x.Fields)
                .ThenInclude(x => x.FieldScores)
                .SingleOrDefaultAsync(x => x.SiteId == siteId);

            if (site == null)
            {
                throw new Exception($"Site {siteId} not found");
            }

            var allExistingAllocations = site.Fields
                .SelectMany(f => f.Allocations
                    .GroupBy(a => new {a.Date, a.ProductType, a.PerTray, a.PerPunnet})
                    .Select(g => new AllocationState(g.Key.Date,
                            new Product
                            {
                                Type = g.Key.ProductType,
                                PerTray = g.Key.PerTray, 
                                PerPunnet = g.Key.PerPunnet
                            }, 
                            g.OrderByDescending(x => x.Version)
                            .FirstOrDefault()
                        )
                    )
                )
                .ToArray();

            var allProducts = GetByPopularity(allExistingAllocations, x => x.Product)
                .ToArray();

            var days = Enumerable.Range(0, 7)
                .Select(x => firstDay.AddDays(x))
                .ToArray();

            var activeAllocations = allExistingAllocations
                .Where(x => x.Weight > 0)
                .Where(x => x.Date.IsBetween(firstDay, lastDay))
                .ToArray();

            var usedProducts = GetByPopularity(activeAllocations, x => x.Product)
                .ToArray();

            return new DailyAllocations
            {
                UsedProducts = usedProducts,
                Allocations = activeAllocations,
                AllProducts = allProducts
            };
        }

        private static IEnumerable<TResult> GetByPopularity<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            return source
                .GroupBy(selector)
                .OrderByDescending(x => x.Count())
                .Select(x => x.Key);
        }

        public async Task SetAllocation(SingleAllocation allocation)
        {
            await _dbContext.Persist(
                x => x.Allocations,
                x => x.Date == allocation.Date &&
                     x.FieldId == allocation.FieldId &&
                     x.ProductType == allocation.Product.Type &&
                     x.PerPunnet == allocation.Product.PerPunnet &&
                     x.PerTray == allocation.Product.PerTray,
                _auditDataProvider,
                (alloc, isNew) =>
                {
                    if (isNew)
                    {
                        alloc.Date = allocation.Date;
                        alloc.FieldId = allocation.FieldId;
                        alloc.ProductType = allocation.Product.Type;
                        alloc.PerPunnet = (int)allocation.Product.PerPunnet;
                        alloc.PerTray = (int)allocation.Product.PerTray;
                    }
                    alloc.Field = null;
                    alloc.Weight = allocation.Weight;
                });
        }
    }
}