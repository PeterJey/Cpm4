using Microsoft.AspNetCore.Mvc;

namespace Cpm.Web.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string GetLocalUrl(this IUrlHelper urlHelper, string localUrl)
        {
            if (!urlHelper.IsLocalUrl(localUrl))
            {
                return urlHelper.Page("/Index");
            }

            return localUrl;
        }

        public static string ResetPasswordCallbackUrl(this IUrlHelper urlHelper, string userId, string code)
        {
            return urlHelper.Page(
                "/Account/ResetPassword",
                pageHandler: null,
                values: new { userId, code },
                protocol: urlHelper.ActionContext.HttpContext.Request.Scheme);
        }

        public static string ApplicationUrl(this IUrlHelper urlHelper)
        {
            return urlHelper.Page(
                "/Dashboard",
                pageHandler: null,
                values: new { },
                protocol: urlHelper.ActionContext.HttpContext.Request.Scheme);
        }

        public static string DashboardUrl(this IUrlHelper urlHelper)
        {
            return urlHelper.Page(
                "/Dashboard",
                pageHandler: null,
                values: new { },
                protocol: urlHelper.ActionContext.HttpContext.Request.Scheme);
        }

        public static string EditUserUrl(this IUrlHelper urlHelper, string userId)
        {
            return urlHelper.Page(
                "/Admin/Users/Edit",
                pageHandler: null,
                values: new { id = userId },
                protocol: urlHelper.ActionContext.HttpContext.Request.Scheme);
        }
    }
}
