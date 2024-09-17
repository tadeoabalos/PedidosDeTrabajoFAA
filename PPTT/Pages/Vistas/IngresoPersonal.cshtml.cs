using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PPTT.Pages.Vistas
{

    public class IngresoPersonalModel : PageModel
    {
        [BindProperty]
        public required int DNI { get; set; }

        [BindProperty]
        public required int NumeroDeControl { get; set; }

        [BindProperty]
        public required string Contraseña { get; set; }

       
        public void OnGet()
        {

        }

        public void OnPost()
        {

        }
    }
}