using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

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
            // Mensajes para comprobar que los valores por defecto están cargados
            Console.WriteLine("OnGet:");
            Console.WriteLine($"DNI: {DNI}");
            Console.WriteLine($"NumeroDeControl: {NumeroDeControl}");
            Console.WriteLine($"ViejaContraseña: {ViejaContraseña}");
            Console.WriteLine($"NuevaContraseña: {NuevaContraseña}");
            Console.WriteLine($"ConfirmacionContraseña: {ConfirmacionContraseña}");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine("Inicio OnPostAsync");
            Console.WriteLine($"DNI: {DNI}");
            Console.WriteLine($"NumeroDeControl: {NumeroDeControl}");
            Console.WriteLine($"ViejaContraseña: {ViejaContraseña}");
            Console.WriteLine($"NuevaContraseña: {NuevaContraseña}");
            Console.WriteLine($"ConfirmacionContraseña: {ConfirmacionContraseña}");

            bool isChanged = await CambiarContraseña(DNI, ViejaContraseña, NuevaContraseña);
            Console.WriteLine("Después de CambiarContraseña");

            if (isChanged)
            {
                Console.WriteLine("Contraseña cambiada correctamente.");
                int _rol = HttpContext.Session.GetInt32("UserRole") ?? 0;
                HttpContext.Session.SetInt32("UserRole", _rol);

                if (_rol < 2)
                {
                    return RedirectToPage("/Index");
                }
                else if (_rol > 1)
                {
                    return Page();
                }
                else
                {
                    return Page();
                }
            }
            else
            {
                Console.WriteLine("Fallo al cambiar la contraseña.");
                ModelState.AddModelError(string.Empty, "Las credenciales no son válidas o no se pudo cambiar la contraseña.");
                return Page();
            }
        }

        private async Task<bool> CambiarContraseña(int dni,  string viejacontraseña, string nuevacontraseña)
        {
            Console.WriteLine("CambiarContraseña - Parámetros recibidos:");
            Console.WriteLine($"DNI: {dni}");
            Console.WriteLine($"ViejaContraseña: {viejacontraseña}");
            Console.WriteLine($"NuevaContraseña: {nuevacontraseña}");

            string connectionString = _configuration.GetConnectionString("ConnectionSQL");
            Console.WriteLine("Connection string: " + connectionString);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    Console.WriteLine("Antes de abrir la conexión");
                    await connection.OpenAsync();
                    Console.WriteLine("Conexión abierta");

                    using (SqlCommand command = new SqlCommand("PrimerIngreso", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@DNI", dni);
                        command.Parameters.AddWithValue("@Contraseña_Vieja", viejacontraseña);
                        command.Parameters.AddWithValue("@Contraseña_Nueva", nuevacontraseña);

                        Console.WriteLine("Antes de ejecutar el stored procedure");
                        Console.WriteLine($"Stored Procedure: PrimerIngreso");
                        Console.WriteLine($"@DNI: {dni}");
                        Console.WriteLine($"@Contraseña_Vieja: {viejacontraseña}");
                        Console.WriteLine($"@Contraseña_Nueva: {nuevacontraseña}");

                        // Ejecuta el stored procedure
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        Console.WriteLine($"Stored procedure ejecutado. Filas afectadas: {rowsAffected}");

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
