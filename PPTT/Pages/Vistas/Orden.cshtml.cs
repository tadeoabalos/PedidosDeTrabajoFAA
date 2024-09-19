using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;


namespace PPTT.Pages.Vistas
{
    public class OrdenModel : PageModel
    {
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

        public void OnGet()
        {

        }

        public IActionResult OnPost()
        {
            return Redirect(HttpContext.Request.Headers["Referer"].ToString());
        }
    }
}

