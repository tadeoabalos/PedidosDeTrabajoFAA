using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PPTT.Data;
using PPTT.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;

namespace PPTT.Pages.Administradores
{
    public class CreateModel : PageModel
    {
        private readonly PPTT.Data.DBPPTTContext _context;

        public CreateModel(PPTT.Data.DBPPTTContext context)
        {
            _context = context;
            Admin = new Admin(); // Inicializa el objeto Admin
        }

        [BindProperty]
        public Admin Admin { get; set; }
        public List<Division> Divisions { get; set; } = new List<Division>();
        public List<Servicio> Servicios { get; set; } = new List<Servicio>();
        public int DNI { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Admin.ID_Rol_Fk = 1; // Asigna el rol
            Admin.Fecha_Baja = new DateTime(1, 1, 1); 
            Admin.Fecha_Alta = DateTime.Now; // Asigna la fecha actual

            // Guarda el objeto Admin en el contexto
            _context.Usuario.Add(Admin);
            await _context.SaveChangesAsync();

            DNI = Admin.DNI; // Asigna el DNI
            HttpContext.Session.SetInt32("DNI", DNI);

            // Lógica para invertir y hashear el DNI
            string numeroStr = DNI.ToString();
            string numeroInvertido = new string(numeroStr.Reverse().ToArray());

            byte[] bytesContraseña = Encoding.ASCII.GetBytes(numeroInvertido);
            byte[] hashContraseña = MD5.HashData(bytesContraseña);
            HttpContext.Session.Set("hashContraseña", hashContraseña);

            return RedirectToPage("/Administradores/SubirPass");
        }


        public async Task<IActionResult> OnGetAsync()
        {
            int _rol = HttpContext.Session.GetInt32("UserRole") ?? 0;

            if (_rol < 2)
            {
                return RedirectToPage("/Index");
            }
            else if (_rol > 1)
            {
                // Aquí solo necesitas obtener las divisiones
                Divisions = await _context.GetDivisionAsync();
                return Page(); // Muestra la página del formulario
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
