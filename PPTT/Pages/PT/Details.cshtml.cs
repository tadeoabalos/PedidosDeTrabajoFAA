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
        public int PrioridadId;
        public List<Motivo> Motivos { get; set; } = new List<Motivo>(); 

        public void RetornaMotivo(int IdPt, int IdEstado)
        {           
            Motivos = _context.Motivo
                .FromSqlRaw("EXEC [dbo].[RetornaMotivo] @p0, @p1", IdPt, IdEstado)
                .ToList(); 
        }

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

            RetornaMotivo(PedidoTrabajo.ID_Orden_Trabajo_Pk, PedidoTrabajo.ID_Estado_Fk);
            return Page();
        }
        

        public async Task<JsonResult> OnGetPrioridadesAsync()
        {
            var prioridades = await _context.GetPrioridadAsync();
            return new JsonResult(prioridades);
        }        
        public async Task<IActionResult> OnPostFinalizarEstadoAsync(int OrdenTrabajoId)
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[FinalizarPedidoTrabajo] @p0", OrdenTrabajoId);
            return RedirectToPage("./Index");
        }
        public async Task<IActionResult> OnPostSuspenderEstadoAsync(int OrdenTrabajoId, DateTime fechaEstimadaFin, string motSus)
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[SuspenderPedidoTrabajo] @p0, @p1, @p2", OrdenTrabajoId, fechaEstimadaFin, motSus);
            return RedirectToPage("./Index");
        }
        public async Task<IActionResult> OnPostEnProcesoEstadoAsync(int OrdenTrabajoId, int IdUsuario)
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[AsignarUsuarioAOrden] @p0, @p1", IdUsuario ,OrdenTrabajoId);
            return RedirectToPage("./Index");
        }       
        public async Task<IActionResult> OnPostCancelarEstadoConMotivoAsync(int OrdenTrabajoId, string motCan)
        {          
            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[CancelarPedidoTrabajo] @p0, @p1", OrdenTrabajoId, motCan);
            return RedirectToPage("./Index");
        }
        public async Task<IActionResult> OnPostSetPrioridadAsync(int OrdenTrabajoId, int PrioridadId) 
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[SetPrioridad] @p0, @p1", OrdenTrabajoId, PrioridadId);
            return RedirectToPage("./Index");
        }
        public async Task<IActionResult> OnPostPendienteEstadoAsync(int OrdenTrabajoId)
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[PendientePedidoTrabajo] @p0", OrdenTrabajoId);
            return RedirectToPage("./Index");
        }        
    }
}

