using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PPTT.Pages.Administradores
{
    public class MenuModel : PageModel
    {
        public IActionResult OnGet()
        {

            int _rol = HttpContext.Session.GetInt32("UserRole") ?? 0;
            HttpContext.Session.SetInt32("UserRole", _rol);

            if (_rol < 2)
            {
                return RedirectToPage("/Index");
            }
            else if (_rol > 1)
            {
                return Page();
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Rol no reconocido.");
                return Page();
            }
        }
        public IActionResult OnPostCerrar()
        {
            HttpContext.Session.SetInt32("UserRole", 0);
            return RedirectToPage("/Index");
        }
    }
}