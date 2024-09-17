using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PPTT.Pages.Vistas
{
    //[Authorize]
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

        public void OnGet()
        {

        }

        public void OnPost()
        {

        }
    }
}