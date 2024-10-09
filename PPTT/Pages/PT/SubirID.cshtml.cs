using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using PPTT.Data;

namespace PPTT.Pages.PT
{
    public class SubirIDModel : PageModel
    {
        private readonly IConfiguration _configuration;

        // Diccionario para almacenar los IDs
        public Dictionary<int, string> Tareas { get; private set; } = new Dictionary<int, string>();

        public SubirIDModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            // Recuperar valores de sesión
            int _id_tarea = HttpContext.Session.GetInt32("tarea") ?? 0;

            // Ejecutar el stored procedure
            bool isExecuted = await EjecutarDiferentesIDs(_id_tarea);

            // Cargar tareas desde la sesión si se ejecutó correctamente
            if (isExecuted)
            {
                string tareasJson = HttpContext.Session.GetString("Tareas");
                if (!string.IsNullOrEmpty(tareasJson))
                {
                    Tareas = JsonConvert.DeserializeObject<Dictionary<int, string>>(tareasJson) ?? new Dictionary<int, string>();
                }
            }

            // Redirigir o mostrar la página según el resultado
            return RedirectToPage("/Index");
        }

        private async Task<bool> EjecutarDiferentesIDs(int idTarea)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionSQL");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("Diferentes_IDs", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@ID_Tarea", idTarea));

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int id = reader.GetInt32(0);
                                string descripcion = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);

                                // Agregar cada tarea al diccionario
                                Tareas[id] = descripcion;
                                Console.WriteLine($"ID: {id}, Descripción: {descripcion}");
                            }
                        }
                    }
                }

                // Solo almacenar las tareas en la sesión si hay datos
                if (Tareas.Count > 0)
                {
                    HttpContext.Session.SetString("Tareas", JsonConvert.SerializeObject(Tareas));
                }

                return Tareas.Count > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
    }
}
