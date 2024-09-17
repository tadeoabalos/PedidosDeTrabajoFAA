using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PPTT.Pages.Vistas
{

    public class IngresoPrimeraVezModel : PageModel
    {
        [BindProperty]
        public required int DNI { get; set; }

        [BindProperty]
        public required int NumeroDeControl { get; set; }

        [BindProperty]
        public required string ViejaContraseña { get; set; }

        [BindProperty]
        public required string NuevaContraseña { get; set; }

        [BindProperty]
        public required string ConfirmacionContraseña { get; set; }

        [BindProperty]
        public required string PermisoParaContinuar { get; set; }


        public void OnGet()
        {

        }

        public IActionResult OnPost()
        {
            return RedirectToPage("/Index");
        }
    }
}
