using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using NuGet.Protocol;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace PPTT.Pages.Vistas
{
    public class IngresoPrimeraVezModel : PageModel
    {
        private readonly IConfiguration _configuration;

        public IngresoPrimeraVezModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public required int DNI { get; set; }

        [BindProperty]
        public required int NumeroDeControl { get; set; }

        [BindProperty]
        public required string ViejaContrase�a { get; set; }

        [BindProperty]
        public required string NuevaContrase�a { get; set; }

        [BindProperty]
        public required string ConfirmacionContrase�a { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //lo hago Bytes
            byte[] bytesContrase�aNueva;
            bytesContrase�aNueva = ASCIIEncoding.ASCII.GetBytes(NuevaContrase�a);
            //lo hasheo
            byte[] hashContrase�aNueva;
            hashContrase�aNueva = MD5.HashData(bytesContrase�aNueva);
            //lo hago Bytes
            byte[] bytesContrase�aVieja;
            bytesContrase�aVieja = ASCIIEncoding.ASCII.GetBytes(ViejaContrase�a);
            //lo hasheo
            byte[] hashContrase�aVieja;
            hashContrase�aVieja = MD5.HashData(bytesContrase�aVieja);
            bool isChanged = await CambiarContrase�a(DNI, hashContrase�aVieja, hashContrase�aNueva);

            if (isChanged)
            {
                return RedirectToPage("/Vistas/IngresoPersonal");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Las credenciales no son v�lidas o no se pudo cambiar la contrase�a.");
                return Page();
            }
        }

        private async Task<bool> CambiarContrase�a(int dni,  byte[] viejacontrase�a, byte[] nuevacontrase�a)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionSQL");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (SqlCommand command = new SqlCommand("PrimerIngreso", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@DNI", dni);
                        command.Parameters.AddWithValue("@Contrase�a_Vieja", viejacontrase�a);
                        command.Parameters.AddWithValue("@Contrase�a_Nueva", nuevacontrase�a);

                        // Ejecuta el stored procedure
                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        // Verifica si se afect� alguna fila
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cambiar la contrase�a: {ex.Message}");
                return false;
            }
        }
    }
}
