using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PPTT.Pages.Vistas
{
    //[Authorize]
    public class DatosPersonalesModel : PageModel
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
        public required string Grado { get; set; }

        [BindProperty]
        public required string Organismo { get; set; }

        [BindProperty]
        public required string DependenciaInterna { get; set; }

        [BindProperty]
        public required string RTI { get; set; }

        [BindProperty]
        public required string NumeroDeOficina { get; set; }

        [BindProperty]
        public required string ColorDeOficina { get; set; }
        public void OnGet()
        {

        }

        public IActionResult OnPost()
        {
            return RedirectToPage("/Index");
        }
    }
}