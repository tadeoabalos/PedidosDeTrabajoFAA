using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using PPTT.Data;
using PPTT.Models;

namespace PPTT.Pages.Administradores
{
    public class CreateModel : PageModel
    {
        private readonly PPTT.Data.DBPPTTContext _context;

        public CreateModel(PPTT.Data.DBPPTTContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Admin Admin { get; set; } = default!;
        public List<Division> Divisions { get; set; } = new List<Division>(); 
        public List<Servicio> Servicios { get; set; } = new List<Servicio>();
            public int ID_Usuario_Pk { get; set; }
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public string Correo { get; set; }
            public int? ID_Division_Fk { get; set; } // Si puede ser null
            public int? ID_Servicio_Fk { get; set; } // Si puede ser null
            public int DNI { get; set; }
            public string Numero_Control { get; set; }
    

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Admin.ID_Rol_Fk = 1;
  
            _context.usuario.Add(Admin);

            await _context.SaveChangesAsync();


            return RedirectToPage("./Index");
        }

      
        public async Task<IActionResult> OnGetAsync()
{

int _rol = HttpContext.Session.GetInt32("UserRole") ?? 0;
HttpContext.Session.SetInt32("UserRole", _rol);

if (_rol < 2)
{
    return RedirectToPage("/Index");
}
else if (_rol > 1)
{
                Divisions = await _context.GetDivisionAsync();
                return Page();
}
else
{
    ModelState.AddModelError(string.Empty, "Rol no reconocido.");
    return Page();
}

    }
        public async Task<JsonResult> OnGetServiciosByDivisionAsync(string division)
        {
            var servicios = await _context.GetServiciosAsync(int.Parse(division));
            return new JsonResult(servicios);
        }
    }
}
