using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PPTT.Data;
using PPTT.Models;
using System.Collections.Generic;
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Protocol;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Diagnostics.Contracts;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Configuration;

namespace PPTT.Pages.Administradores
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly PPTT.Data.DBPPTTContext _context;
        private int datos;
        private IConfiguration? configuration;
        public IndexModel(PPTT.Data.DBPPTTContext context)
        {
            _context = context;
            _configuration = configuration; // Asegúrate de que esto esté presente
        }
        public List<ServicioModel> Items { get; set; } = new List<ServicioModel>();
        public IList<Admin> Admin { get;set; } = default!;
        public class ServicioModel
        {
            public int? ID_Servicio_Fk { get; set; }
        }
        private async Task<bool> CrearContraStoredProcedure(int DNI, byte[] Contra)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionSQL");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("Crear_Password", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        //ejecuto el stored procedure 
                        command.Parameters.AddWithValue("@DNI", DNI);
                        command.Parameters.AddWithValue("@Pass", Contra);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
        public async Task<IActionResult> OnGetAsync()
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
                Admin = await _context.usuario.ToListAsync();
                int DNI = HttpContext.Session.GetInt32("DNI") ?? 0;
                byte[] hashContraseña = HttpContext.Session.Get("hashContraseña");
                await CrearContraStoredProcedure(DNI, hashContraseña);
                return Page();
            }
        }
    }
}
