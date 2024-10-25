
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PPTT.Models;


namespace PPTT.Pages.Administradores
{
    public class DeleteModel : PageModel
    {
        private readonly PPTT.Data.DBPPTTContext _context;

        public DeleteModel(PPTT.Data.DBPPTTContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Admin Admin { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            int _rol = HttpContext.Session.GetInt32("UserRole") ?? 0;

            if (_rol < 2)
            {
                return RedirectToPage("/Index");
            }
            else if (_rol > 1)
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
            else
            {
                ModelState.AddModelError(string.Empty, "Rol no reconocido.");
                return Page();
            }
        }                                                                                           
                                                                                                                                                                                                                                                                                                                        
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Usuario.FindAsync(id);
            if (admin != null)
            {
                Admin = admin;
                // Supongamos que 'Admin' es el usuario que deseas modificar
                var usuario = Admin; // Asegúrate de tener el ID del usuario

                if (usuario != null)
                {
                    usuario.Fecha_Baja = DateTime.Now; ; // Cambia la fecha de baja
                    _context.SaveChanges(); // Guarda los cambios en la base de datos
                }

                await _context.SaveChangesAsync();  
            }

            return RedirectToPage("./Index");
        }
    }
}
