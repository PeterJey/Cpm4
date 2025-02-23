using Cpm.Core.Services.Fields;
using Optional;

namespace Cpm.Web.Helpers
{
    public interface ICalendarPositionResolver
    {
        Option<int> Resolve(FieldDetails field, string which);
    }
}