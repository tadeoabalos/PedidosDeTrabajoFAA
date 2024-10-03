
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PPTT.Pages.Administradores
{
    public class EmpleadosModel : PageModel
    {

        [BindProperty]
        public required string Nombre { get; set; }

        [BindProperty]
        public required string Apellido { get; set; }

        [BindProperty]
        public required string Correo { get; set; }

        [BindProperty]
        public required int NumeroDeControl { get; set; }

        [BindProperty]
        public required string Division { get; set; }

        [BindProperty]
        public required string Servicio { get; set; }

        [BindProperty]
        public required string Rol { get; set; }

        //public IActionResult OnGet()
        //{
        //    int _rol = HttpContext.Session.GetInt32("UserRole") ?? 0;
        //    HttpContext.Session.SetInt32("UserRole", _rol);

        //    if (_rol < 2)
        //    {
        //        return RedirectToPage("/Index");
        //    }
        //    else if (_rol > 1)
        //    {
        //        return Page();
        //    }
        //    else
        //    {
        //        ModelState.AddModelError(string.Empty, "Rol no reconocido.");
        //        return Page();
        //    }
        //}
    }
}
