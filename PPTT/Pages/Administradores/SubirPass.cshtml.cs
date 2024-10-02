using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using PPTT.Data;
using PPTT.Models;

namespace PPTT.Pages.Administradores
{
    public class SubirPassModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public SubirPassModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            int DNI = HttpContext.Session.GetInt32("DNI") ?? 0;
            byte[] hashContraseña = HttpContext.Session.Get("hashContraseña");
            await CrearContraStoredProcedure(DNI, hashContraseña);
            return RedirectToPage("/Administradores/Index");
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
    }
}
