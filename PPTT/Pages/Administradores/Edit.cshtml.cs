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
    public class EditModel : PageModel
    {
        private readonly PPTT.Data.DBPPTTContext _context;

        public EditModel(PPTT.Data.DBPPTTContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Admin Admin { get; set; } = default!;
        public List<Division> Divisions { get; set; } = new List<Division>();
        public List<Servicio> Servicios { get; set; } = new List<Servicio>();

        public async Task<JsonResult> OnGetServiciosByDivisionAsync(string division)
        {
            var servicios = await _context.GetServiciosAsync(int.Parse(division));
            return new JsonResult(servicios);
        }

        // Método que se ejecuta cuando se carga la página
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            Divisions = await _context.GetDivisionAsync();

            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.usuario.FirstOrDefaultAsync(m => m.ID_Usuario_Pk == id);
            if (admin == null)
            {
                return NotFound();
            }

            Admin = admin;
            return Page();
        }

        // Método que se ejecuta cuando se hace envío del formulario
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Cargar los valores originales de ID_Servicio_Fk y ID_Password_Fk si no se han modificado en el formulario
            var adminFromDb = await _context.usuario.AsNoTracking().FirstOrDefaultAsync(m => m.ID_Usuario_Pk == Admin.ID_Usuario_Pk);
            if (adminFromDb == null)
            {
                return NotFound();
            }

            // Asegurar que los campos que no se editan conserven su valor original
            Admin.ID_Servicio_Fk = adminFromDb.ID_Servicio_Fk;  // Mantener el servicio original si no se modifica
            Admin.ID_Password_Fk = adminFromDb.ID_Password_Fk;  // Mantener la contraseña original si no se modifica

            // Marcar explícitamente los campos modificados
            _context.Attach(Admin).Property(a => a.Nombre).IsModified = true;
            _context.Attach(Admin).Property(a => a.DNI).IsModified = true;
            // Solo marca más campos si son modificados por el usuario
            // _context.Attach(Admin).Property(a => a.OtroCampo).IsModified = true;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdminExists(Admin.ID_Usuario_Pk))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool AdminExists(int id)
        {
            return _context.usuario.Any(e => e.ID_Usuario_Pk == id);
        }
    }
}
