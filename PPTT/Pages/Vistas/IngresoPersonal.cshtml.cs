using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PPTT.Models;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace PPTT.Pages.Vistas
{
    public class IngresoPersonalModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private int _rol;
        private string? _nombre;
        private string? _apellido;
        private int _ingreso;
        private int _idusuario;
        private int _division;
        private int _division2;// Añadida la declaración de _division

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

        public IActionResult OnGet()
        {
            int _rol = HttpContext.Session.GetInt32("UserRole") ?? 0;
            HttpContext.Session.SetInt32("UserRole", _rol);
            Console.WriteLine(_rol);

            if (_rol == 1)
            {
                return RedirectToPage("/PT/VerTrabajo");
            }
            else if (_rol == 2)
            {
                return RedirectToPage("/Vistas/IndexAdmin");
            }
            else if (_rol == 3)
            {
                return RedirectToPage("/Vistas/IndexLogueado");
            }
            else
            {
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            byte[] bytesContraseña = Encoding.ASCII.GetBytes(Contraseña);
            byte[] hashContraseña = MD5.HashData(bytesContraseña);
            bool isValid = await EjecutarValidarStoredProcedure(DNI, NumeroDeControl, hashContraseña);

            if (isValid)
            {
                HttpContext.Session.SetInt32("UserRole", _rol);
                HttpContext.Session.SetInt32("Division", _division);
                HttpContext.Session.SetInt32("Division2", _division2);
                HttpContext.Session.SetInt32("IDUsuario", _idusuario);
                HttpContext.Session.SetString("Nombre", _nombre);
                HttpContext.Session.SetString("Apellido", _apellido);

                if (_ingreso == 0)
                {
                    return RedirectToPage("/Vistas/IngresoPrimeraVez");
                }
                else if (_rol == 1)
                {
                    return RedirectToPage("/PT/VerTrabajo");
                }
                else if (_rol == 2)
                {
                    return RedirectToPage("/Vistas/IndexAdmin");
                }
                else if (_rol == 3)
                {
                    return RedirectToPage("/Vistas/IndexLogueado");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Rol no reconocido.");
                    return Page();
                }
            }
            else
            {
                // Mostrar un mensaje de error si las credenciales son incorrectas
                ModelState.AddModelError(string.Empty, "DNI, Número de control o Contraseña incorrectos.");
                TempData["CredencialesFallidas"] = true;
                return Page();
            }
        }


        private async Task<bool> EjecutarValidarStoredProcedure(int dni, int numeroDeControl, byte[] password)
        {
            string? connectionString = _configuration.GetConnectionString("ConnectionSQL");

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
                                _rol = reader.GetInt32(0);
                                _nombre = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                                _ingreso = reader.IsDBNull(2) ? 0 : reader.GetInt32(2);
                                _division = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
                                _idusuario = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);
                                _division2 = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);
                                _apellido = reader.IsDBNull(6) ? string.Empty : reader.GetString(6);
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
