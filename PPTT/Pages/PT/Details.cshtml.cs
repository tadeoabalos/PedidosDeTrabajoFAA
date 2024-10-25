using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using PPTT.Models;

namespace PPTT.Pages.PT
{
    public class DetailsPPTT : PageModel
    {
        private readonly PPTT.Data.DBPPTTContext _context;

        public DetailsPPTT(PPTT.Data.DBPPTTContext context)
        {
            _context = context;
        }

        public List<Admin> Usuarios { get; set; } = new List<Admin>();
        public PTUsuario? PedidoTrabajo { get; set; } = default!;
        public List<Estado> Estado { get; set; } = new List<Estado>();
        public List<Prioridad> Prioridad { get; set; } = new List<Prioridad>();
        public string MotivoSuspension { get; set; } = string.Empty; // Inicializar como cadena vacía

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Intentar obtener los datos del pedido de trabajo
            PedidoTrabajo = await _context.PTUsuario
                .Include(pt => pt.Organismo)
                .Include(pt => pt.Tarea)
                .Include(pt => pt.Estado)
                .Include(pt => pt.Dependencia_Interna)
                .Include(pt => pt.Grado)
                .Include(pt => pt.Prioridad)
                .FirstOrDefaultAsync(m => m.ID_Orden_Trabajo_Pk == id);

            // Verifica si PedidoTrabajo es nulo antes de acceder a sus propiedades
            if (PedidoTrabajo == null)
            {
                return NotFound();
            }

            // Ahora es seguro acceder a las propiedades de PedidoTrabajo
            HttpContext.Session.SetString("Correo", PedidoTrabajo.Correo ?? string.Empty);

            // Carga los usuarios y estados
            Usuarios = await _context.GetUsuariosFiltradosByOrdenAsync(id) ?? new List<Admin>();
            Estado = await _context.GetEstadosAsync() ?? new List<Estado>();
            Prioridad = await _context.GetPrioridadAsync() ?? new List<Prioridad>();
            return Page();
        }
        public int IdUsuario;

        public async Task<JsonResult> OnGetPrioridadesAsync()
        {
            var prioridades = await _context.GetPrioridadAsync();
            return new JsonResult(prioridades);
        }

        public async Task<IActionResult> OnPostFinalizarEstadoAsync(int ordenTrabajoId)
        {
            // Almacenar ID_Estado_Fk en la sesión
            if (PedidoTrabajo != null)
            {
                HttpContext.Session.SetInt32("ID_Estado_Fk", PedidoTrabajo.ID_Estado_Fk);
            }

            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[FinalizarPedidoTrabajo] @p0", ordenTrabajoId);
            return RedirectToPage("./MandarMailCambioEstado");
        }

        public async Task<IActionResult> OnPostSuspenderEstadoAsync(int ordenTrabajoId, string motivoSuspension, DateTime? fechaEstimadaFin)
        {
            // Verificar que FechaEstimadaFin no sea null
            if (fechaEstimadaFin.HasValue)
            {
                // Almacenar la fecha en formato string en la sesión
                string fechaString = fechaEstimadaFin.Value.ToString("dd-MM-yyyy"); // Cambia el formato según tus necesidades
                HttpContext.Session.SetString("FechaEstimadaFin", fechaString);
            }

            // Almacenar el motivo en la sesión
            HttpContext.Session.SetString("motivo", motivoSuspension);

            // Almacenar ID_Estado_Fk en la sesión
            if (PedidoTrabajo != null)
            {
                HttpContext.Session.SetInt32("ID_Estado_Fk", PedidoTrabajo.ID_Estado_Fk);
            }

            // Ejecutar el stored procedure para suspender el pedido de trabajo
            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[SuspenderPedidoTrabajo] @p0, @p1", ordenTrabajoId, fechaEstimadaFin);

            // Redirigir a la página de confirmación o donde sea necesario
            return RedirectToPage("./MandarMailCambioEstado");
        }

        public async Task<IActionResult> OnPostAsignarUsuarioEstadoAsync(int ordenTrabajoId, int idUsuario)
        {
            // Almacenar ID_Estado_Fk en la sesión
            if (PedidoTrabajo != null)
            {
                HttpContext.Session.SetInt32("ID_Estado_Fk", PedidoTrabajo.ID_Estado_Fk);
            }

            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[AsignarUsuarioAOrden] @p0, @p1", idUsuario, ordenTrabajoId);
            return RedirectToPage("./MandarMailCambioEstado");
        }

        public async Task<IActionResult> OnPostCancelarPedidoEstadoAsync(int ordenTrabajoId, string motivoCancelacion)
        {
            // Almacenar el motivo de cancelación en la sesión
            HttpContext.Session.SetString("motivo", motivoCancelacion);

            // Almacenar ID_Estado_Fk en la sesión
            if (PedidoTrabajo != null)
            {
                HttpContext.Session.SetInt32("ID_Estado_Fk", PedidoTrabajo.ID_Estado_Fk);
            }

            // Ejecutar el stored procedure para cancelar el pedido de trabajo
            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[CancelarPedidoTrabajo] @p0", ordenTrabajoId);

            // Redirigir a la página de confirmación o donde sea necesario
            return RedirectToPage("./MandarMailCambioEstado");
        }

        public async Task<IActionResult> OnPostPonerPendienteEstadoAsync(int ordenTrabajoId)
        {
            // Almacenar ID_Estado_Fk en la sesión
            if (PedidoTrabajo != null)
            {
                HttpContext.Session.SetInt32("ID_Estado_Fk", PedidoTrabajo.ID_Estado_Fk);
            }

            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[PendientePedidoTrabajo] @p0", ordenTrabajoId);
            return RedirectToPage("./MandarMailCambioEstado");
        }
    }
}
