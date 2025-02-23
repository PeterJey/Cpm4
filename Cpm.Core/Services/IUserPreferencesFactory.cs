using System.Collections.Generic;
using Cpm.Core.ViewModels;

namespace Cpm.Core.Services
{
    public interface IUserPreferencesFactory
    {
        UserPreferences Create();
        ICollection<OptionVm> AvailableAreaUnits();
        ICollection<OptionVm> AvailableAllocationUnits();
    }
}