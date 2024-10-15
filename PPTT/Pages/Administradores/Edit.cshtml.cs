using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public List<SelectListItem> Roles { get; set; } = new List<SelectListItem>();

        public async Task<JsonResult> OnGetServiciosByDivisionAsync(string division)
        {
            if (int.TryParse(division, out int divisionId))
            {
                var servicios = await _context.GetServiciosAsync(divisionId);
                return new JsonResult(servicios.Select(s => new
                {
                    s.ID_Servicio_Pk,
                    s.Descripcion_Servicio
                }));
            }
            return new JsonResult(new List<object>());
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            Divisions = await _context.GetDivisionAsync();
            Roles = Enum.GetValues(typeof(Admin.Rol))
                .Cast<Admin.Rol>()
                .Select(c => new SelectListItem
                {
                    Value = ((int)c).ToString(),
                    Text = c.ToString()
                }).ToList();
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Usuario.FirstOrDefaultAsync(m => m.ID_Usuario_Pk == id);
            if (admin == null)
            {
                return NotFound();
            }

            Admin = admin;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var adminFromDb = await _context.Usuario.AsNoTracking().FirstOrDefaultAsync(m => m.ID_Usuario_Pk == Admin.ID_Usuario_Pk);
            if (adminFromDb == null)
            {
                return NotFound();
            }

            Admin.ID_Servicio_Fk = adminFromDb.ID_Servicio_Fk;
            Admin.ID_Password_Fk = adminFromDb.ID_Password_Fk;

            // Guardar el rol nuevo en la propiedad ID_Rol_Fk del objeto Admin
            Admin.ID_Rol_Fk = Admin.ID_Rol_Fk ?? 0; // Asegúrate de que ID_Rol_Fk tenga un valor predeterminado si es nulo

            // Marcar las propiedades que han sido modificadas
            _context.Attach(Admin).Property(a => a.Nombre).IsModified = true;
            _context.Attach(Admin).Property(a => a.DNI).IsModified = true;
            _context.Attach(Admin).Property(a => a.ID_Rol_Fk).IsModified = true; // Marcar ID_Rol_Fk como modificado

            try
            {
                await _context.SaveChangesAsync(); // Guardar los cambios en la base de datos
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdminExists(Admin.ID_Usuario_Pk))
                {
                    return NotFound();
                }
                else
                {
                    throw; // Lanzar la excepción para manejarla más arriba si es necesario
                }
            }

            return RedirectToPage("./Index"); // Redirigir a la página de índice después de guardar
        }


        private bool AdminExists(int id)
        {
            return _context.Usuario.Any(e => e.ID_Usuario_Pk == id);
        }
    }
}
