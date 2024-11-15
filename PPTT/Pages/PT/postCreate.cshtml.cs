using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PPTT.Pages.PT
{
    public class postCreateModel : PageModel
    {
        public void OnGet()
        {
            ViewData["ShowHeader"] = false;
        }
    }
}
