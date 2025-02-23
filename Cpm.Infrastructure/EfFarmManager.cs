using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cpm.Core;
using Cpm.Core.Farms;
using Cpm.Core.Models;
using Cpm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cpm.Infrastructure
{
    public class EfFarmManager : IFarmManager
    {
        private readonly ApplicationDbContext _dbContext;

        public EfFarmManager(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateNewFarm(FarmModel model)
        {
            await _dbContext
                .Farms
                .AddAsync(new Farm
                {
                    FarmId = IdHelper.NewId(),
                    FirstDayOfYear = DateTime.ParseExact(model.FirstDayOfYear, "yyyy-MM-dd", null),
                    Name = model.Name,
                    Sites = CreateSites(model.Sites),
                });
            await _dbContext.SaveChangesAsync();
        }

        public Task<ICollection<Farm>> GetAllFarms()
        {
            return _dbContext.Farms
                .Include(x => x.Sites)
                .ThenInclude(x => x.Fields)
                .Include(x => x.Sites)
                .ThenInclude(x => x.Scenarios)
                .ToListAsync()
                .ContinueWith(t => t.Result as ICollection<Farm>);
        }

        public Task<ICollection<Site>> GetAllSites()
        {
            return GetSites();
        }

        private ICollection<Site> CreateSites(IEnumerable<SiteModel> siteModels)
        {
            return siteModels
                .Select((site, index) => new Site
                {
                    SiteId = IdHelper.NewId(),
                    Name = site.Name,
                    Postcode = site.Postcode,
                    Fields = CreateFields(site.Fields),
                    Order = index,
                })
                .ToList();
        }

        private ICollection<Field> CreateFields(IEnumerable<FieldModel> fieldModels)
        {
            return fieldModels
                .Select((field, index) => new Field
                {
                    FieldId = IdHelper.NewId(),
                    Name = field.Name,
                    Variety = field.Variety,
                    AreaInHectares = field.AreaInHectares,
                    Order = index,
                })
                .ToList();
        }

        private Task<ICollection<Site>> GetSites(Func<IQueryable<Site>, IQueryable<Site>> modifierFunc = null)
        {
            return (modifierFunc == null
                    ? _dbContext.Sites
                    : modifierFunc(_dbContext.Sites)
                )
                .Include(x => x.Farm)
                .Include(x => x.Fields)
                .Include(x => x.Scenarios)
                .OrderBy(x => x.Order)
                .ToListAsync()
                .ContinueWith(t => t.Result as ICollection<Site>);
        }
    }
}