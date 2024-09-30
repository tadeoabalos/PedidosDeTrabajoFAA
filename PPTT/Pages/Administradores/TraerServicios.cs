using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace PPTT.Pages.Administradores
{
    public class TraerServicios
    {
        private SqlDataReader reader;

        public object ModelState { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            bool isValid = await EjecutarValidarStoredProcedure(HttpContext, reader, GetReader());
            if (isValid)
            {
                HttpContext.Session.SetInt32("serv", _servicio);
                return RedirectToPage("/Administradores/Index");
            }
            else
            {
                object value = ModelState.AddModelError(string.Empty, "Las credenciales no son válidas.");
                return Page();
            }
        }

        private IActionResult Page()
        {
            throw new NotImplementedException();
        }

        private SqlDataReader GetReader()
        {
            return reader;
        }

        private async Task<bool> EjecutarValidarStoredProcedure(HttpContext httpContext, SqlDataReader reader, SqlDataReader reader)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionSQL");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("Servicios_Traer", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        //ejecuto el stored procedure de login con estos valores
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            var _id = 0;
                            var _servicio = "";
                            _id = reader.GetInt32(0);
                            _servicio = reader(1);
                            httpContext.Session.SetInt32("serv", _servicio);
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



