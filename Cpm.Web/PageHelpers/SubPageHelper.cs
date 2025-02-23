using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cpm.Web.PageHelpers
{
    public static class SubPageHelper
    {
        public static string ClassForSubPage(this ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                             ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}