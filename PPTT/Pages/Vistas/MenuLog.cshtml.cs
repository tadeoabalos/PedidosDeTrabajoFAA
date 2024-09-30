using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PPTT.Pages.Vistas
{
    public class MenuLogModel : PageModel
    {
        public void OnGet()
        {
        }
        public IActionResult OnPostCerrar()
        {
            HttpContext.Session.SetInt32("UserRole", 0);
            return RedirectToPage("/Index");
        }
    }
}
