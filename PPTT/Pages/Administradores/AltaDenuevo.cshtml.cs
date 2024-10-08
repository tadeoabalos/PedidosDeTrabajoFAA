using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PPTT.Models;

namespace PPTT.Pages.Administradores
{
    public class AltaDenuevoModel : PageModel
    {
        private readonly PPTT.Data.DBPPTTContext _context;

        public AltaDenuevoModel(PPTT.Data.DBPPTTContext context)
        {
            _context = context;
        }
        public Admin Admin { get; set; } = default!;
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Usuario.FindAsync(id);
            if (admin != null)
            {
                Admin = admin;
                var usuario = Admin; 

                if (usuario != null)
                {
                    usuario.Fecha_Baja = new DateTime(1, 1, 1);  
                    _context.SaveChanges(); 
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
