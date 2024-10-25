using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PPTT.Data;
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
        public List<Estado> Estado { get; set; } = default!;
        public List<Prioridad> Prioridad { get; set; } = new List<Prioridad>();
        public string MotivoSuspension { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {            
            if (id == null)
            {
                return NotFound();
            }
            else 
            {
                Usuarios = await _context.GetUsuariosFiltradosByOrdenAsync(id);
            }
            HttpContext.Session.SetString("Correo", PedidoTrabajo.Correo);
            HttpContext.Session.SetInt32("ID_Estado_Fk", PedidoTrabajo.ID_Estado_Fk);

            Estado = await _context.GetEstadosAsync();
            Prioridad = await _context.GetPrioridadAsync();
            PedidoTrabajo = await _context.PTUsuario
                .Include(pt => pt.Organismo) 
                .Include(pt => pt.Tarea) 
                .Include(pt => pt.Estado) 
                .Include(pt => pt.Dependencia_Interna) 
                .Include(pt => pt.Grado)
                .Include(pt => pt.Prioridad)
                .FirstOrDefaultAsync(m => m.ID_Orden_Trabajo_Pk == id); 

            if (PedidoTrabajo == null)
            {
                return NotFound();
            }

            return Page();
        }

        public int IdUsuario;

        public async Task<JsonResult> OnGetPrioridadesAsync()
        {
            var prioridades = await _context.GetPrioridadAsync();
            return new JsonResult(prioridades);
        }        

        public async Task<IActionResult> OnPostFinalizarEstadoAsync(int OrdenTrabajoId)
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[FinalizarPedidoTrabajo] @p0", OrdenTrabajoId);
            return RedirectToPage("./MandarMailCambioEstado");
        }
        public async Task<IActionResult> OnPostSuspenderEstadoAsync(int OrdenTrabajoId, string MotivoSuspension, DateTime? FechaEstimadaFin)
        {
            // Verificar que FechaEstimadaFin no sea null
            if (FechaEstimadaFin.HasValue)
            {
                // Almacenar la fecha en formato string en la sesión
                string fechaString = FechaEstimadaFin.Value.ToString("dd-MM-yyyy"); // Cambia el formato según tus necesidades
                HttpContext.Session.SetString("FechaEstimadaFin", fechaString);
            }

            // Almacenar el motivo en la sesión
            HttpContext.Session.SetString("motivo", MotivoSuspension);

            // Ejecutar el stored procedure para suspender el pedido de trabajo
            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[SuspenderPedidoTrabajo] @p0, @p1", OrdenTrabajoId, FechaEstimadaFin);

            // Redirigir a la página de confirmación o donde sea necesario
            return RedirectToPage("./MandarMailCambioEstadodex");
        }
        public async Task<IActionResult> OnPostEnProcesoEstadoAsync(int OrdenTrabajoId, int IdUsuario)
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[AsignarUsuarioAOrden] @p0, @p1", IdUsuario ,OrdenTrabajoId);
            return RedirectToPage("./MandarMailCambioEstado");
        }
        public async Task<IActionResult> OnPostCancelarEstadoAsync(int OrdenTrabajoId, string motivoCancelacion)
        {
            // Almacenar el motivo de cancelación en la sesión
            HttpContext.Session.SetString("motivo", motivoCancelacion);

            // Ejecutar el stored procedure para cancelar el pedido de trabajo
            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[CancelarPedidoTrabajo] @p0", OrdenTrabajoId);

            // Redirigir a la página de confirmación o donde sea necesario
            return RedirectToPage("./MandarMailCambioEstadodex");
        }
        public async Task<IActionResult> OnPostPendienteEstadoAsync(int OrdenTrabajoId)
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[PendientePedidoTrabajo] @p0", OrdenTrabajoId);
            return RedirectToPage("./MandarMailCambioEstado");
        }
        

    }
}
