using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cpm.Core.Models;
using Cpm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cpm.Web.Security
{
    public class SitePermissionManager : ISitePermissionManager
    {
        private readonly ApplicationDbContext _dbContext;

        public SitePermissionManager(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<ICollection<SiteUserPermission>> GetForUserAsync(string userId)
        {
            return await _dbContext.SiteUserPermissions
                .Include(x => x.Site)
                .ThenInclude(x => x.Farm)
                .Include(x => x.Site)
                .ThenInclude(x => x.Fields)
                .Where(x => x.UserId == userId)
                .ToListAsync()
                .ContinueWith(t => t.Result as ICollection<SiteUserPermission>);
        }

        public async Task AddOrUpdate(SiteUserPermission permission)
        {
            if (await _dbContext.SiteUserPermissions
                .AnyAsync(x => x.UserId == permission.UserId && x.SiteId == permission.SiteId))
            {
                _dbContext.Entry(permission).State = EntityState.Modified;
            }
            else
            {
                _dbContext.Entry(permission).State = EntityState.Added;
            }
            await _dbContext.SaveChangesAsync();
        }

        public Task Clear(SiteUserPermission permission)
        {
            _dbContext.Entry(permission).State = EntityState.Deleted;
            return _dbContext.SaveChangesAsync();
        }
    }
}