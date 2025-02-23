namespace Cpm.Web.PageHelpers
{
    public class ExternalLoginProviderHelper
    {
        public static string GetFaClassForProvider(string provider)
        {
            switch ((provider ?? "").ToLowerInvariant())
            {
                case "google":
                    return "fa-google";
                case "microsoft":
                    return "fa-windows";
                case "facebook":
                    return "fa-facebook";
            }
            return "fa-external-link";
        }
    }
}