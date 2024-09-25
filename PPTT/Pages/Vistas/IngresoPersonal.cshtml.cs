using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace PPTT.Pages.Vistas
{
    public class IngresoPersonalModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private int _rol;
        private string _nombre; // Nueva propiedad para almacenar el nombre
        private int _ingreso; // Nueva propiedad para almacenar el ingreso

        public IngresoPersonalModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
        public required int DNI { get; set; }

        [BindProperty]
        public required int NumeroDeControl { get; set; }

        [BindProperty]
        public required string Contraseña { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            bool isValid = await EjecutarValidarStoredProcedure(DNI, NumeroDeControl, Contraseña);

            if (isValid)
            {
                HttpContext.Session.SetInt32("UserRole", _rol);
                HttpContext.Session.SetString("UserName", _nombre);
                Console.WriteLine($"Rol: {_rol}, Nombre: {_nombre}, Ingreso: {_ingreso}");

                if (_ingreso == 0) // Verifica si ingreso es 0
                {
                    return RedirectToPage("/Vistas/IngresoPrimeraVez");
                }

                if (_rol < 2)
                {
                    string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                    Console.WriteLine(ipAddress);
                    return RedirectToPage("/Vistas/MenuLog");
                }
                else if (_rol > 1)
                {
                    return RedirectToPage("/Administradores/Menu");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Rol no reconocido.");
                    return Page();
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Las credenciales no son válidas.");
                return Page();
            }
        }

        private async Task<bool> EjecutarValidarStoredProcedure(int dni, int numeroDeControl, string password)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionSQL");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("Validar", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@DNI", dni);
                        command.Parameters.AddWithValue("@Numero_Control", numeroDeControl);
                        command.Parameters.AddWithValue("@Password", password);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                // Obtener el rol, nombre e ingreso
                                _rol = reader.GetInt32(0);
                                _nombre = reader.IsDBNull(1) ? string.Empty : reader.GetString(1); // Captura el nombre
                                _ingreso = reader.IsDBNull(2) ? 0 : reader.GetInt32(2); // Captura el ingreso
                                Console.WriteLine($"Rol: {_rol}, Nombre: {_nombre}, Ingreso: {_ingreso}");
                                return _rol != 0;
                            }
                            else
                            {
                                _rol = 0;
                                return false;
                            }
                        }
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
