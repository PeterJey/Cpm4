using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cpm.Web.Pages
{
    public class AboutModel : PageModel
    {
        public string Message { get; set; }

        public void OnGet()
        {
        }
    }
}
