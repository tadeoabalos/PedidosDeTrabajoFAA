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

                if (_rol < 2)
                {
                    return RedirectToPage("/Index");
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
                        command.Parameters.AddWithValue("@Numero_De_Control", numeroDeControl);
                        command.Parameters.AddWithValue("@Password", password);

                        SqlParameter rolParam = new SqlParameter("@Rol", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(rolParam);

                        await command.ExecuteNonQueryAsync();
                        _rol = (int)rolParam.Value;

                        return _rol != 0;
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
