using System;
using System.Threading.Tasks;
using Cpm.Core.Services.Fields;
using Cpm.Core.ViewModels;

namespace Cpm.Core.Services.Diary
{
    public interface IDiaryManager
    {
        Task<DiaryDayVm> GetDayDetailsVm(string fieldId, DateTime date);
        Task<ViewModels.DiaryVm> GetDiaryVm(string fieldId, int newPosition, DateTime firstDay, int weeks);
        Task<WeeklyOverviewVm> GetWeeklyOverviewVm(SiteDetails snaps, int newPosition, DateTime firstDay);
    }
}