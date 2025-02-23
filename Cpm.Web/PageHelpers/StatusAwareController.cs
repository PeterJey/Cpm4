using Microsoft.AspNetCore.Mvc;

namespace Cpm.Web.PageHelpers
{
    public abstract class StatusAwareController : Controller
    {
        private const string TempDataKey = "StatusMessageHtml";

        public string LoadStatusHtml()
        {
            return TempData[TempDataKey]?.ToString() ?? string.Empty;
        }

        protected IBlankStatusMessageBuilder SaveStatus()
        {
            return new StatusMessageBuilder(x => TempData[TempDataKey]=x.GetHtml());
        }
    }
}