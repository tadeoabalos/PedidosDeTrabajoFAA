using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PPTT.Data;
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
                    usuario.Fecha_Baja = new DateTime(1, 1, 1); // Cambia la fecha de baja
                    _context.SaveChanges(); // Guarda los cambios en la base de datos
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
