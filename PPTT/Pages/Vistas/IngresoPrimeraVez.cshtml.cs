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
        public required string ViejaContraseña { get; set; }

        [BindProperty]
        public required string NuevaContraseña { get; set; }

        [BindProperty]
        public required string ConfirmacionContraseña { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //lo hago Bytes
            byte[] bytesContraseñaNueva;
            bytesContraseñaNueva = ASCIIEncoding.ASCII.GetBytes(NuevaContraseña);
            //lo hasheo
            byte[] hashContraseñaNueva;
            hashContraseñaNueva = MD5.HashData(bytesContraseñaNueva);
            //lo hago Bytes
            byte[] bytesContraseñaVieja;
            bytesContraseñaVieja = ASCIIEncoding.ASCII.GetBytes(ViejaContraseña);
            //lo hasheo
            byte[] hashContraseñaVieja;
            hashContraseñaVieja = MD5.HashData(bytesContraseñaVieja);
            bool isChanged = await CambiarContraseña(DNI, hashContraseñaVieja, hashContraseñaNueva);

            if (isChanged)
            {
                return RedirectToPage("/Vistas/IngresoPersonal");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Las credenciales no son válidas o no se pudo cambiar la contraseña.");
                return Page();
            }
        }

        private async Task<bool> CambiarContraseña(int dni,  byte[] viejacontraseña, byte[] nuevacontraseña)
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
                        command.Parameters.AddWithValue("@Contraseña_Vieja", viejacontraseña);
                        command.Parameters.AddWithValue("@Contraseña_Nueva", nuevacontraseña);

                        // Ejecuta el stored procedure
                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        // Verifica si se afectó alguna fila
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // Muestra el error en la consola para depuración
                Console.WriteLine($"Error al cambiar la contraseña: {ex.Message}");
                return false;
            }
        }
    }
}
