using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cpm.Web.Pages
{
    public class ContactModel : PageModel
    {
        public string Message { get; set; }

        public void OnGet()
        {
        }
    }
}
