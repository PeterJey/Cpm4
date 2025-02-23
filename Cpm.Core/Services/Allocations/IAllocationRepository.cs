using System;
using System.Threading.Tasks;

namespace Cpm.Core.Services.Allocations
{
    public interface IAllocationRepository
    {
        Task<DailyAllocations> GetForSiteAndDates(string siteId, DateTime firstDay, DateTime lastDay);
        Task SetAllocation(SingleAllocation allocation);
    }
}