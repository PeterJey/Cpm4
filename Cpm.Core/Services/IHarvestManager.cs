using System;
using System.Threading.Tasks;

namespace Cpm.Core.Services
{
    public interface IHarvestManager
    {
        Task SetHarvestedWeight(string fieldId, DateTime date, decimal? value, bool last);
        Task SetPlannedPick(string fieldId, DateTime date, decimal? value);
    }
}