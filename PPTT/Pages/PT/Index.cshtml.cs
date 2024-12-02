using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using PPTT.Models;
using X.PagedList.Extensions;
using Microsoft.Data.SqlClient;
using System.Data;

namespace PPTT.Pages.Administradores
{
    public class IndexPPTT : PageModel
    {
        private readonly PPTT.Data.DBPPTTContext _context;

        public IndexPPTT(PPTT.Data.DBPPTTContext context)
        {
            _context = context;
        }

        public IPagedList<PTUsuario> PedidoTrabajo { get; set; } = default!;
        public List<Admin> Usuarios { get; set; } = new List<Admin>();
        public PTUsuario PT { get; set; } = default!;
        public List<Prioridad> Prioridad { get; set; } = new List<Prioridad>();
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int PageNumber { get; set; } = 1;

        public async Task OnGetAsync(int? pageNumber, DateTime? fechaInicio, DateTime? fechaFin)
        {
            int _rol = HttpContext.Session.GetInt32("UserRole") ?? 0;
            if (_rol > 1)
            {
                PageNumber = pageNumber ?? 1;
                Prioridad = await _context.GetPrioridadAsync();

                var pedidosQuery = _context.PTUsuario
                    .Include(pt => pt.Organismo)
                    .Include(pt => pt.Tarea)
                    .Include(pt => pt.Estado)
                    .Include(pt => pt.Prioridad)
                    .Include(pt => pt.Dependencia_Interna)
                    .Include(pt => pt.Grado)
                    .Include(pt => pt.Division)
                    .AsNoTracking()
                    .Where(pt => pt.ID_Estado_Fk != 1004 && pt.ID_Estado_Fk != 1006);
                if (fechaInicio.HasValue && fechaFin.HasValue)
                {
                    var fechaFinFinal = fechaFin.Value.Date.AddDays(1).AddTicks(-1);
                    pedidosQuery = pedidosQuery.Where(pt => pt.Fecha_Subida >= fechaInicio && pt.Fecha_Subida <= fechaFinFinal);
                }
                PedidoTrabajo = pedidosQuery.ToPagedList(PageNumber, 8);
            }
            else if (_rol == 2)
            {
                RedirectToPage("/Administradores/IndexAdmin");
            }
            else
            {
                RedirectToPage("/Index");
            }
        }

        public async Task<JsonResult> OnGetUsuariosFiltradosAsync(string division)
        {
            var usuarios = await _context.GetUsuariosFiltradosAsync(int.Parse(division));
            return new JsonResult(usuarios);
        }

        public async Task<JsonResult> OnGetUsuarioPorPtAsync(int idPedidoTrabajo)
        {
            var usuario = await _context.GetUsuarioPorPtAsync(idPedidoTrabajo);
            if (usuario == null)
            {
                return new JsonResult(new { success = false, message = "Usuario no encontrado" });
            }

            return new JsonResult(new { success = true, nombre = usuario[0].Nombre, apellido = usuario[0].Apellido });
        }


        public async Task<IActionResult> OnPostAsignarUsuarioAsync(int UsuarioId, int OrdenTrabajoId)
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC AsignarUsuarioAOrden @p0, @p1", UsuarioId, OrdenTrabajoId);
            return RedirectToPage("/PT/Index");
        }

        public async Task<IActionResult> OnPostSetPrioridadAsync(int OrdenTrabajoId, int PrioridadId)
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[SetPrioridad] @p0, @p1", OrdenTrabajoId, PrioridadId);
            return RedirectToPage("/PT/Index");
        }

        public async Task<JsonResult> OnGetPrioridadesAsync()
        {
            var prioridades = await _context.GetPrioridadAsync();
            return new JsonResult(prioridades);
        }

        public IActionResult OnGetBuscarPedidos(string searchTerm)
        {
            var resultados = new List<dynamic>();
            
            using (var connection = new SqlConnection(_context.Database.GetDbConnection().ConnectionString))
            {
                connection.Open();
                
                using (var command = new SqlCommand("BuscarPedidos", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@searchTerm", searchTerm);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            resultados.Add(new
                            {
                                idOrden = reader["ID_Orden_Fk"],
                                fechaSubida = reader["Fecha_Subida"],
                                grado = reader["ID_Grado_Fk"],
                                organismo = reader["ID_Organismo_Fk"],
                                apellidoSolicitante = reader["Apellido_Solicitante"],
                                correo = reader["Correo"],
                                rti = reader["RTI_Solicitante"],
                                oficina = reader["Numero_Oficina_PT"],
                                division = reader["ID_Division_Fk"],
                                tarea = reader["ID_Tarea_Fk"],
                                observacion = reader["Observacion"],
                                estado = reader["ID_Estado_Fk"],
                                prioridad = reader["ID_Prioridad_Fk"],                                
                            });
                        }
                    }
                }
            }

            return new JsonResult(resultados);
        }

    }
}
