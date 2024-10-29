using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace PPTT.Pages.PT
{
    public class MandarMailCambioEstadoModel : PageModel
    {
        private readonly IConfiguration _configuration;

        // Constructor para inyectar IConfiguration
        public MandarMailCambioEstadoModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult OnGet()
        {
            // Tu l�gica para procesar el formulario

            // Redirigir a la p�gina anterior usando el encabezado Referer
            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer))
            {
                return Redirect(referer);
            }

            // Si no hay referer, redirige a una p�gina predeterminada
            return RedirectToPage("");
        }

    }
}
