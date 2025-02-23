using Microsoft.AspNetCore.Builder;

namespace Cpm.Infrastructure.AjaxHttpStatusCodes
{
    public static class AjaxStatusCodesExtensions
    {
        public static IApplicationBuilder DisableStatusCodesForAjax(this IApplicationBuilder app)
        {
            return app.UseMiddleware<AjaxStatusCodesMiddleware>();
        }
    }
}
