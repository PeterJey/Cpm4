using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Optional;

namespace Cpm.Web.Pages.Errors
{
    public class ErrorModel : PageModel
    {
        public Option<string> RequestId { get; set; }

        public void OnGet()
        {
            RequestId = Activity.Current
                .SomeNotNull()
                .Map(x => x.Id)
                .Filter(x => !string.IsNullOrWhiteSpace(x))
                .Or(HttpContext.TraceIdentifier);
        }
    }
}
