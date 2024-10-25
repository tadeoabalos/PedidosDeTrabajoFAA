
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

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
            int _rol = HttpContext.Session.GetInt32("UserRole") ?? 0;
            HttpContext.Session.SetInt32("UserRole", _rol);

            if (_rol < 2)
            {
                return RedirectToPage("/Index");
            }
            else if (_rol > 1)
            {
                int DNI = HttpContext.Session.GetInt32("DNI") ?? 0;
                Console.WriteLine("DNI del usuario: " + DNI);

                // Convertir el DNI a string, invertirlo y crear el hash MD5
                string numeroStr = DNI.ToString();
                string numeroInvertido = new string(numeroStr.Reverse().ToArray());
                byte[] bytesContraseña = Encoding.ASCII.GetBytes(numeroInvertido);
                byte[] hashContraseña = MD5.HashData(bytesContraseña);

                Console.WriteLine("Hash de la contraseña (en bytes): " + BitConverter.ToString(hashContraseña));

                // Llamar al stored procedure para crear o actualizar la contraseña
                bool isValid = await CrearContraStoredProcedure(DNI, hashContraseña);
                if (isValid)
                {
                    Console.WriteLine("Contraseña actualizada correctamente.");
                    return RedirectToPage("/Administradores/Index");
                }
                else
                {
                    Console.WriteLine("Error al actualizar la contraseña.");
                    return Page();
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Rol no reconocido.");
                return Page();
            }
        }

        private async Task<bool> CrearContraStoredProcedure(int DNI, byte[] hashContraseña)
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
                        // Pasar los parámetros al stored procedure
                        command.Parameters.AddWithValue("@DNI", DNI);
                        command.Parameters.AddWithValue("@Pass", hashContraseña);  // Enviar el hash en formato byte[]

                        // Ejecutar el stored procedure
                        await command.ExecuteNonQueryAsync();
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
