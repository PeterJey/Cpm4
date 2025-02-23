using System;
using System.Linq;
using System.Threading.Tasks;
using Cpm.Core.Models;
using Cpm.Core.Services;
using Cpm.Core.Services.Fields;
using Cpm.Core.Services.Notes;
using Cpm.Core.Services.Serialization;
using Cpm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Cpm.Infrastructure.New
{
    public class EfFieldRepository : IFieldRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IYieldSerializer _yieldSerializer;
        private readonly IWeightsSerializer _weightsSerializer;
        private readonly IAuditDataProvider _auditDataProvider;
        private readonly IPictureMetadataSerializer _pictureSerializer;

        public EfFieldRepository(
            ApplicationDbContext dbContext, 
            IYieldSerializer yieldSerializer, 
            IWeightsSerializer weightsSerializer,
            IAuditDataProvider auditDataProvider,
            IPictureMetadataSerializer pictureSerializer
            )
        {
            _dbContext = dbContext;
            _yieldSerializer = yieldSerializer;
            _weightsSerializer = weightsSerializer;
            _auditDataProvider = auditDataProvider;
            _pictureSerializer = pictureSerializer;
        }

        public async Task<FieldDetails> GetFieldById(string fieldId)
        {
            var field = await _dbContext.Fields
                .Include(x => x.Site)
                .ThenInclude(x => x.Farm)
                .Include(x => x.PinnedNotes)
                .Include(x => x.HarvestRegisters)
                .Include(x => x.FieldScores)
                .Include(x => x.PickingPlans)
                .SingleOrDefaultAsync(x => x.FieldId == fieldId);

            if (field == null)
            {
                return null;
            }

            return new FieldDetails(field, _yieldSerializer, _weightsSerializer, _pictureSerializer);
        }

        public async Task<SiteDetails> GetSiteById(string siteId)
        {
            var site = await _dbContext.Sites
                .Include(x => x.Fields)
                .ThenInclude(x => x.PinnedNotes)
                .Include(x => x.Fields)
                .ThenInclude(x => x.HarvestRegisters)
                .Include(x => x.Fields)
                .ThenInclude(x => x.FieldScores)
                .Include(x => x.Fields)
                .ThenInclude(x => x.PickingPlans)
                .Include(x => x.Fields)
                .ThenInclude(x => x.Site)
                .ThenInclude(x => x.Farm)
                .SingleOrDefaultAsync(x => x.SiteId == siteId);

            if (site == null)
            {
                return null;
            }

            return new SiteDetails(site, _yieldSerializer, _weightsSerializer, _pictureSerializer);
        }

        public async Task SaveFieldNote(string fieldId, DateTime date, Action<PinnedNote> updateAction)
        {
            var note = (await _dbContext.PinnedNotes
                    .Where(x => x.FieldId == fieldId && x.Date == date.Date)
                    .OrderByDescending(n => n.Version)
                    .FirstOrDefaultAsync()
                )
                .Some()
                .NotNull()
                .Map(n =>
                {
                    _dbContext.Entry(n).State = EntityState.Detached;

                    n.Version++;
                    return n;
                })
                .ValueOr(new PinnedNote
                {
                    Date = date,
                    FieldId = fieldId,
                    Version = 1,
                });

            note.CreatedBy = _auditDataProvider.GetUserField();
            note.CreatedOn = _auditDataProvider.GetTimestampField();

            updateAction(note);

            await _dbContext.PinnedNotes.AddAsync(note);
            await _dbContext.SaveChangesAsync();
        }
    }
}