using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PPTT.Pages.Vistas
{
    public class YaLogModel : PageModel
    {

        public IActionResult OnGet()
        {
            int _rol = HttpContext.Session.GetInt32("UserRole") ?? 0;
            HttpContext.Session.SetInt32("UserRole", _rol);

            if (_rol == 0)
            {
                return RedirectToPage("/Vistas/IngresoPersonal");
            }
            else
            {
                return Page(); 
            }
        }

        public IActionResult OnPost()
        {
            HttpContext.Session.SetInt32("UserRole", 0);
            return RedirectToPage("/Vistas/MenuLog");
        }
    }
}
