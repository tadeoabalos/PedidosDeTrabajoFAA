
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using PPTT.Data;

namespace PPTT.Pages.Administradores
{
    public class TraerServicioModel : PageModel
    {
        private readonly IConfiguration _configuration;

        // Diccionario para almacenar los servicios
        public Dictionary<int, string> Servicios { get; private set; } = new Dictionary<int, string>();

        public TraerServicioModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Recuperar valores de sesión
            int _id_servicio_pk = HttpContext.Session.GetInt32("id_servicio") ?? 0;
            string _descripcion_servicio = HttpContext.Session.GetString("_descripcion_servicio") ?? string.Empty;

            // Solo se establece si realmente hay un valor
            HttpContext.Session.SetInt32("id_servicio", _id_servicio_pk);
            HttpContext.Session.SetString("_descripcion_servicio", _descripcion_servicio);

            // Cargar servicios desde la sesión
            string serviciosJson = HttpContext.Session.GetString("Servicios");
            if (!string.IsNullOrEmpty(serviciosJson))
            {
                Servicios = JsonConvert.DeserializeObject<Dictionary<int, string>>(serviciosJson) ?? new Dictionary<int, string>();
            }

            Console.WriteLine($"ID: {_id_servicio_pk}");
            Console.WriteLine($"Descripción: {_descripcion_servicio}");

            // Ejecutar el stored procedure
            bool isValid = await EjecutarValidarStoredProcedure();

            // Redirigir o mostrar la página según el resultado
            return isValid ? RedirectToPage("/Administradores/Index") : Page();
        }

        private async Task<bool> EjecutarValidarStoredProcedure()
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

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int id_servicio_pk = reader.GetInt32(0);
                                string descripcion_servicio = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);

                                // Agregar cada servicio al diccionario
                                Servicios[id_servicio_pk] = descripcion_servicio;
                                Console.WriteLine($"ID: {id_servicio_pk}, Descripción: {descripcion_servicio}");
                            }
                        }
                    }
                }

                // Solo almacenar los servicios en la sesión si hay datos
                if (Servicios.Count > 0)
                {
                    HttpContext.Session.SetObject("Servicios", Servicios);
                }

                return Servicios.Count > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
    }
}
