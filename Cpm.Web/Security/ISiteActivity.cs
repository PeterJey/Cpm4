using Cpm.Core.Models;

namespace Cpm.Web.Security
{
    public interface ISiteActivity
    {
        bool ForSite(string siteId);
        bool ForSite(Site site);
    }
}