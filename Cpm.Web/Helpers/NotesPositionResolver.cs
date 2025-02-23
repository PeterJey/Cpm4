using System;
using System.Linq;
using Cpm.Core.Services.Diary;
using Cpm.Core.Services.Fields;
using Optional;
using Optional.Collections;

namespace Cpm.Web.Helpers
{
    public class NotesPositionResolver : ICalendarPositionResolver
    {
        private readonly IDiaryRangeCalculator _calculator;
        private readonly ICalendarPositionResolver _next;

        public NotesPositionResolver(IDiaryRangeCalculator calculator, ICalendarPositionResolver next)
        {
            _calculator = calculator;
            _next = next;
        }

        public Option<int> Resolve(FieldDetails field, string which)
        {
            return (
                    !string.IsNullOrEmpty(which)
                        ? which.Equals("firstnote", StringComparison.OrdinalIgnoreCase)
                            ? field.Notes.OrderBy(x => x.Date).FirstOrNone()
                            : which.Equals("lastnote", StringComparison.OrdinalIgnoreCase)
                                ? field.Notes.OrderBy(x => x.Date).LastOrNone()
                                : Option.None<NoteDetails>()
                        : Option.None<NoteDetails>()
                )
                .Map(x => _calculator.GetPositionForDate(field.FirstWeekCommencing, x.Date))
                .Else(_next.Resolve(field, which));
        }
    }
}