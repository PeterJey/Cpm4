using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cpm.Core.Models;
using Cpm.Core.Services;
using Cpm.Core.Services.Fields;
using Cpm.Core.Services.Forecast;
using Cpm.Core.Services.Serialization;
using Cpm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Optional;

namespace Cpm.Infrastructure
{
    public class EfFieldManager : IFieldManager
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IYieldSerializer _yieldSerializer;
        private readonly IAuditDataProvider _auditDataProvider;

        public EfFieldManager(
            ApplicationDbContext dbContext,
            IYieldSerializer yieldSerializer,
            IAuditDataProvider auditDataProvider
            )
        {
            _dbContext = dbContext;
            _yieldSerializer = yieldSerializer;
            _auditDataProvider = auditDataProvider;
        }

        public Task<Field> GetField(string fieldId)
        {
            return _dbContext.Fields
                .Include(x => x.PinnedNotes)
                .Include(x => x.Site)
                .ThenInclude(x => x.Farm)
                .SingleOrDefaultAsync(x => x.FieldId == fieldId);
        }

        public async Task<ICollection<HarvestDay>> GetHarvestedWeight(string fieldId)
        {
            var register = await _dbContext.HarvestRegisters
                .OrderByDescending(x => x.Version)
                .FirstOrDefaultAsync(x => x.FieldId == fieldId);

            if (register != null)
            {
                var days = JsonConvert.DeserializeObject<HarvestDay[]>(register.SerializedValues);

                if (days != null)
                {
                    return days;
                }
            }

            return new List<HarvestDay>();
        }

        public Task<bool> UpdateActiveScenario(string fieldId, string scenarioId)
        {
            return UpdateFieldScore(fieldId, score => { score.ActiveScenarioId = scenarioId; });
        }

        public Task<bool> UpdateDescription(string fieldId, string description)
        {
            return UpdateFieldScore(fieldId, score => { score.Description = description; });
        }

        public Task<bool> UpdateBudget(string fieldId, Yield yield)
        {
            return UpdateFieldScore(fieldId, score => { score.SerializedBudget = _yieldSerializer.Serialize(yield); });
        }

        public async Task<bool> UpdateProfile(string fieldId, string profileName)
        {
            var field = await _dbContext.Fields.FirstOrDefaultAsync(x => x.FieldId == fieldId);

            if (field == null)
            {
                return false;
            }

            field.ProfileName = profileName;

            _dbContext.Entry(field).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync();

            return true;
        }

        private async Task<bool> UpdateFieldScore(string fieldId, Action<FieldScore> updateAction)
        {
            var field = await _dbContext.Fields
                .Include(x => x.FieldScores)
                .SingleOrDefaultAsync(x => x.FieldId == fieldId);

            if (field == null)
            {
                return false;
            }

            var score = field.FieldScores
                .OrderByDescending(x => x.Version)
                .FirstOrDefault()
                .SomeNotNull()
                .Map(x =>
                {
                    ++x.Version;
                    return x;
                })
                .ValueOr(() => new FieldScore
                {
                    FieldId = fieldId,
                    Version = 1,
                });

            score.CreatedBy = _auditDataProvider.GetUserField();
            score.CreatedOn = _auditDataProvider.GetTimestampField();

            updateAction(score);

            _dbContext.Entry(score).State = EntityState.Added;
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}