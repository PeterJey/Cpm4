using System;
using System.Collections.Generic;
using System.Linq;
using Cpm.Core.Extensions;
using Cpm.Core.Models;
using Cpm.Core.Services.Forecast;
using Cpm.Core.Services.Notes;
using Cpm.Core.Services.Serialization;
using Optional;

namespace Cpm.Core.Services.Fields
{
    public class FieldDetails
    {
        public string FieldId { get; }
        public string FieldName { get; }
        public string SiteId { get; }
        public string SiteName { get; }
        public string FarmName { get; }
        public string Variety { get; }
        public string Description { get; }
        public string Postcode { get; }
        public string ProfileName { get; }
        public DateTime FirstWeekCommencing { get; }
        public decimal AreaInHectares { get; }
        public Yield Budget { get; }

        public HarvestDayCollection HarvestHistory { get; }
        public HarvestDayCollection PickingPlan { get; }

        public IReadOnlyCollection<NoteDetails> Notes { get; }

        public string ActiveScenarioId { get; }
        
        public int Index { get; }

        public decimal TotalWeight => HarvestHistory.Days.Sum(x => x.Weight);

        public FieldDetails(
            Field field, 
            IYieldSerializer yieldSerializer, 
            IWeightsSerializer weightsSerializer, 
            IPictureMetadataSerializer pictureSerializer
            )
        {
            if (field == null) throw new ArgumentNullException(nameof(field));

            var score = field.FieldScores
                .OrderBy(x => x.Version)
                .LastOrDefault();

            var harvest = field.HarvestRegisters
                .OrderBy(x => x.Version)
                .LastOrDefault();

            var plan = field.PickingPlans
                .OrderBy(x => x.Version)
                .LastOrDefault();

            var notes = field.PinnedNotes
                .GroupBy(x => x.Date)
                .Select(x => NoteDetails.FromPinnedNote(x, pictureSerializer))
                .IgnoreNulls();

            FieldId = field.FieldId;
            FieldName = field.Name;
            SiteId = field.SiteId;
            SiteName = field.Site.Name;
            FarmName = field.Site.Farm.Name;
            Index = field.Order;
            Variety = field.Variety;
            Description = score?.Description;
            Postcode = field.Site.Postcode;
            ProfileName = field.ProfileName;
            FirstWeekCommencing = field.Site.Farm.FirstDayOfYear;
            AreaInHectares = field.AreaInHectares;
            Budget = yieldSerializer.Deserialize(score?.SerializedBudget);

            HarvestHistory = DeserializeRegister(harvest, FirstWeekCommencing, weightsSerializer);
            PickingPlan = DeserializeRegister(plan, FirstWeekCommencing, weightsSerializer);

            Notes = notes.ToArray();

            ActiveScenarioId = score?.ActiveScenarioId;
        }

        private HarvestDayCollection DeserializeRegister(
            SerializedValuesRegister register,
            DateTime firstWeekCommencing,
            IWeightsSerializer weightsSerializer
            )
        {
            return new HarvestDayCollection(
                register?.FirstDay,
                weightsSerializer.Deserialize(register?.SerializedValues),
                firstWeekCommencing
            );
        }

        public decimal? GetProgress(decimal weight)
        {
            return (Budget.KgPerHectare * AreaInHectares)
                .Some()
                .Filter(x => x != 0)
                .Map(x => weight / x)
                .ToNullable();
        }

        public decimal? GetProgress()
        {
            return GetProgress(TotalWeight);
        }
    }
}
