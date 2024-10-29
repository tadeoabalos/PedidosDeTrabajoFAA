
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
                byte[] bytesContrase�a = Encoding.ASCII.GetBytes(numeroInvertido);
                byte[] hashContrase�a = MD5.HashData(bytesContrase�a);

                Console.WriteLine("Hash de la contrase�a (en bytes): " + BitConverter.ToString(hashContrase�a));

                // Llamar al stored procedure para crear o actualizar la contrase�a
                bool isValid = await CrearContraStoredProcedure(DNI, hashContrase�a);
                if (isValid)
                {
                    Console.WriteLine("Contrase�a actualizada correctamente.");
                    return RedirectToPage("/Administradores/Index");
                }
                else
                {
                    Console.WriteLine("Error al actualizar la contrase�a.");
                    return Page();
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Rol no reconocido.");
                return Page();
            }
        }

        private async Task<bool> CrearContraStoredProcedure(int DNI, byte[] hashContrase�a)
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
                        // Pasar los par�metros al stored procedure
                        command.Parameters.AddWithValue("@DNI", DNI);
                        command.Parameters.AddWithValue("@Pass", hashContrase�a);  // Enviar el hash en formato byte[]

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
