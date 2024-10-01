using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using PPTT.Data;
using PPTT.Models;

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
            int _id_servicio_pk = HttpContext.Session.GetInt32("id_servicio") ?? 0;
            string _descripcion_servicio = HttpContext.Session.GetString("_descripcion_servicio") ?? string.Empty;

            HttpContext.Session.SetInt32("id_servicio", _id_servicio_pk);
            HttpContext.Session.SetString("_descripcion_servicio", _descripcion_servicio);

            string serviciosJson = HttpContext.Session.GetString("Servicios");
            if (!string.IsNullOrEmpty(serviciosJson))
            {
                Servicios = JsonConvert.DeserializeObject<Dictionary<int, string>>(serviciosJson);
            }

            Console.WriteLine($"ID: {_id_servicio_pk}");
            Console.WriteLine($"Descripción: {_descripcion_servicio}");

            bool isValid = await EjecutarValidarStoredProcedure();

            if (isValid)
            {
                return RedirectToPage("/Administradores/Index");
            }
            else
            {
                return Page();
            }
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

                                // agregar cada servicio al diccionario
                                Servicios[id_servicio_pk] = descripcion_servicio;
                                HttpContext.Session.SetObject("Servicios", Servicios);

                                Console.WriteLine($"ID: {id_servicio_pk}, Descripción: {descripcion_servicio}");
                            }
                            return Servicios.Count > 0;
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
