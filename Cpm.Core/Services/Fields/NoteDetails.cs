using System;
using System.Collections.Generic;
using System.Linq;
using Cpm.Core.Models;
using Cpm.Core.Services.Notes;

namespace Cpm.Core.Services.Fields
{
    public class NoteDetails
    {
        public static NoteDetails FromPinnedNote(IEnumerable<PinnedNote> pinnedNotes, IPictureMetadataSerializer serializer)
        {
            if (pinnedNotes == null) throw new ArgumentNullException(nameof(pinnedNotes));
            var localNotes = pinnedNotes.ToArray();

            var current = localNotes.Last();

            if (!localNotes.Any() || localNotes.Last().IsDeleted)
            {
                return null;
            }

            var first = localNotes
                .Reverse()
                .TakeWhile(x => !x.IsDeleted)
                .Last();

            return new NoteDetails(
                current.Text, 
                current.Date.Date, 
                serializer.Deserialize(current.SerializedPictureMetadata),
                first.CreatedOn, 
                first.CreatedBy, 
                current.CreatedOn,
                current.CreatedBy, 
                current.Version - first.Version
                );
        }

        public string Text { get; }
        public DateTime Date { get; }
        public DateTime CreatedOn { get; }
        public string CreatedBy { get; }
        public DateTime ModifiedOn { get; }
        public string ModifiedBy { get; }
        public int ModifiedTimes { get; }
        public IReadOnlyCollection<PictureMetadata> Pictures { get; set; }

        private NoteDetails(string text, DateTime date, IEnumerable<PictureMetadata> pictures, DateTime createdOn, string createdBy, 
            DateTime modifiedOn, string modifiedBy, int modifiedTimes)
        {
            Text = text;
            Date = date;
            Pictures = pictures.ToArray();
            CreatedOn = createdOn;
            CreatedBy = createdBy;
            ModifiedOn = modifiedOn;
            ModifiedBy = modifiedBy;
            ModifiedTimes = modifiedTimes;
        }
    }
}