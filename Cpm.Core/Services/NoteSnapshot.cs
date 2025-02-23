using System;
using Cpm.Core.Models;

namespace Cpm.Core.Services
{
   [Obsolete("check why")]
    public class NoteSnapshot
    {
        public NoteSnapshot(PinnedNote note)
        {
            if (note == null) throw new ArgumentNullException(nameof(note));

            Date = note.Date;
            Text = note.Text;
            ModificationCount = note.Version - 1;
            LastEditor = note.CreatedBy;
            SavedOn = note.CreatedOn;
        }

        public DateTime SavedOn { get; }

        public string LastEditor { get; }

        public int ModificationCount { get; }

        public string Text { get; }

        public DateTime Date { get; }
    }
}