using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Cpm.Infrastructure.AjaxHttpStatusCodes
{
    public class AjaxStatusCodesMiddleware
    {
        private readonly RequestDelegate _next;

        public AjaxStatusCodesMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            var feature = context.Features.Get<IStatusCodePagesFeature>();

            if (feature != null && context.Request.IsAjaxRequest())
            {
                feature.Enabled = false;
            }

            return _next(context);
        }
    }
}
