using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Cpm.Core.Models;
using Cpm.Core.Services;
using Cpm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cpm.Infrastructure.Extensions
{
    public static class ApplicationDbContextExtensions
    {
        public static async Task Persist<TEntity>(this ApplicationDbContext dbContext,
            Func<ApplicationDbContext, DbSet<TEntity>> setSelector,
            Expression<Func<TEntity, bool>> predicate,
            IAuditDataProvider auditDataProvider,
            Action<TEntity, bool> action) where TEntity : class, IVersionable, new()
        {
            var entity = await setSelector(dbContext)
                .OrderByDescending(x => x.Version)
                .FirstOrDefaultAsync(predicate);

            if (entity == null)
            {
                entity = new TEntity
                {
                    Version = 1
                };

                action.Invoke(entity, true);
            }
            else
            {
                dbContext
                    .Entry(entity)
                    .State = EntityState.Detached;

                entity.Version++;

                action.Invoke(entity, false);
            }

            entity.CreatedBy = auditDataProvider.GetUserField();
            entity.CreatedOn = auditDataProvider.GetTimestampField();

            dbContext
                .Entry(entity)
                .State = EntityState.Added;

            await dbContext.SaveChangesAsync();
        }
    }
}
