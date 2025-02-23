using Cpm.Core.Services.Fields;
using Optional;

namespace Cpm.Web.Helpers
{
    public class IntegerPositionResolver : ICalendarPositionResolver
    {
        private readonly ICalendarPositionResolver _next;

        public IntegerPositionResolver(ICalendarPositionResolver next)
        {
            _next = next;
        }

        public Option<int> Resolve(FieldDetails field, string which)
        {
            if (int.TryParse(which, out var position))
            {
                return position.Some();
            }

            return _next.Resolve(field, which);
        }
    }
}