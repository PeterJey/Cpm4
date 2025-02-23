using System.Collections.Generic;
using System.Threading.Tasks;
using Cpm.Core.Models;

namespace Cpm.Web.Security
{
    public interface ISitePermissionManager
    {
        Task<ICollection<SiteUserPermission>> GetForUserAsync(string userId);
        Task AddOrUpdate(SiteUserPermission permission);
        Task Clear(SiteUserPermission permission);
    }
}