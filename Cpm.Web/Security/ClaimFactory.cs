using System.Security.Claims;

namespace Cpm.Web.Security
{
    public static class ClaimFactory
    {
        public static Claim Create(string claimType, string claimValue = null)
        {
            return new Claim(claimType, claimValue);
        }
    }
}
