using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;


namespace PPTT.Pages.Vistas
{
    public class OrdenModel : PageModel
    {
        [BindProperty]
        public required string Apellido { get; set; }

        [BindProperty]
        public required string Nombre { get; set; }

        [BindProperty]
        public required string Correo { get; set; }

        [BindProperty]
        public required string NumeroDeControl { get; set; }

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

        [BindProperty]
        public required string PisoOficina { get; set; }

        [BindProperty]
        public required string DivisionDeMantenimiento { get; set; }

        [BindProperty]
        public required string ServicioRequerido { get; set; }

        [BindProperty]
        public required string DetalleServicio { get; set; }

        [BindProperty]
        public required string Observacion { get; set; }

        // Propiedades para la oficina con problemas
        [BindProperty]
        public required string NumeroDeOficinaProblema { get; set; }

        [BindProperty]
        public required string ColorDeOficinaProblema { get; set; }

        [BindProperty]
        public required string PisoOficinaProblema { get; set; }

        public void OnGet()
        {

        }

        public IActionResult OnPost()
        {
            return Redirect(HttpContext.Request.Headers["Referer"].ToString());
        }
    }
}

