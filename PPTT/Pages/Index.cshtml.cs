using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PPTT.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        //public IActionResult OnGet()
        //{
        //    int _rol = HttpContext.Session.GetInt32("UserRole") ?? 0;
        //    HttpContext.Session.SetInt32("UserRole", _rol);

        //    if (_rol == 1)
        //    {
        //        return RedirectToPage("Vistas/MenuLog");
        //    }
        //    else if (_rol > 1)
        //    {
        //        return RedirectToPage("/Administradores/Menu");
        //    }
        //    else
        //    {
        //        ModelState.AddModelError(string.Empty, "Rol no reconocido.");
        //        return Page();
        //    }
        //}
    }
}
