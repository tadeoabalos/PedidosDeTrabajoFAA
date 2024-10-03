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

            var admin = await _context.Usuario.FirstOrDefaultAsync(m => m.ID_Usuario_Pk == id);
            if (admin == null)
            {
                return NotFound();
            }
            Admin = admin;

            int DniAnterior = Admin.DNI;
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
                if (!AdminExists(Admin.ID_Usuario_Pk))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            //await EditarPW(Admin.DNI, 125);

            return RedirectToPage("./Index");
        }

        private bool AdminExists(int id)
        {
            return _context.Usuario.Any(e => e.ID_Usuario_Pk == id);
        }

        /*public async Task EditarPW(int dni_nuevo, int dni_anterior)
        {     
            await _context.Database.ExecuteSqlRawAsync(
                    "EXEC [dbo].[Editar_Primera_PW] @DNI_ANTERIOR = {0}, @DNI_NUEVO = {1}", dni_anterior, dni_nuevo);           
        }*/
    }
}