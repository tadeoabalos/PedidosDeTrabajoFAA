
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PPTT.Models;

namespace PPTT.Pages.Administradores
{
    public class DetailsModel : PageModel
    {
        private readonly PPTT.Data.DBPPTTContext _context;

        public DetailsModel(PPTT.Data.DBPPTTContext context)
        {
            _context = context;
        }

        public Admin Admin { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            int _rol = HttpContext.Session.GetInt32("UserRole") ?? 0;
            HttpContext.Session.SetInt32("UserRole", _rol);

            if (_rol == 2)
            {
                int datos = HttpContext.Session.GetInt32("datos") ?? 0;
                HttpContext.Session.SetInt32("datos", datos);
                if (datos == 0)
                {
                    datos = datos + 1;
                    HttpContext.Session.SetInt32("datos", datos);
                    Console.WriteLine(datos);
                    return RedirectToPage("/Administradores/TraerServicio");
                }
                else
                {
                    if (id == null)
                    {
                        return NotFound();
                    }

                    var admin = await _context.Usuario.FirstOrDefaultAsync(m => m.ID_Usuario_Pk == id);
                    if (admin == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        Admin = admin;
                    }
                    return Page();
                }
            }
            else
            {
                return RedirectToPage("/Vistas/MenuLog");
            }
        }
    }
}
