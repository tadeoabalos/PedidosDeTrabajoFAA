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
        public List<Servicio> Servicios { get; set; } = new List<Servicio>(); 

        public async Task<JsonResult> OnGetServiciosByDivisionAsync(string division)
        {
             var servicios = await _context.GetServiciosAsync(int.Parse(division));
             return new JsonResult(servicios);
        }
        //Método que se ejecuta cuando se carga la página
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            Divisions = await _context.GetDivisionAsync();

            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Usuarios.FirstOrDefaultAsync(m => m.Id == id);
            if (admin == null)
            {
                return NotFound();
            }
            Admin = admin;
            
            return Page();
        }

        // MÉTODO QUE SE EJECUTA CUANDO SE HACE ENVIO DE FORMULARIO
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Admin).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdminExists(Admin.Id))
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
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}