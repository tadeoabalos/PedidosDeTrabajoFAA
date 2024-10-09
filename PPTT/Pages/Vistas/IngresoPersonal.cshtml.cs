using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PPTT.Pages.Vistas
{
    public class IngresoPersonalModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private int _rol;
        private string? _nombre;
        private int _ingreso;

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
            //si el rol es mayor a 0 significa que esta loggeado asi que hago que no pueda volver a la pagina de loggeo y sea redireccionado a una pagina para cerrar sesion
            int _rol = HttpContext.Session.GetInt32("UserRole") ?? 0;
            HttpContext.Session.SetInt32("UserRole", _rol);
            Console.WriteLine(_rol);
            if (_rol > 0)
            {
                return RedirectToPage("/Vistas/YaLog");
            }
            else
            {
                return Page();
            }

        }
        public async Task<IActionResult> OnPostAsync()
        {
            //lo hago Bytes
            byte[] bytesContraseña;
            bytesContraseña = ASCIIEncoding.ASCII.GetBytes(Contraseña);
            //lo hasheo
            byte[] hashContraseña;
            hashContraseña = MD5.HashData(bytesContraseña);
            Console.WriteLine(hashContraseña);
            bool isValid = await EjecutarValidarStoredProcedure(DNI, NumeroDeControl, hashContraseña);

            if (isValid)
            {
                HttpContext.Session.SetInt32("UserRole", _rol);
                HttpContext.Session.SetString("UserName", _nombre);
                Console.WriteLine($"Rol: {_rol}, Nombre: {_nombre}, Ingreso: {_ingreso}");

                //lo llevo a una pagina hecha para cambiar su contraseña para sacar la predeterminada
                if (_ingreso == 0)
                {
                    return RedirectToPage("/Vistas/IngresoPrimeraVez");
                }
                //decido a que menu lo mando, si al normal o al de admin
                if (_rol < 2)
                {
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
                        //ejecuto el stored procedure de login con estos valores
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
