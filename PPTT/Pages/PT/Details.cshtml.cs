using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        public string motivo { get; set; }
        public List<Estado> Estado { get; set; } = default!;
        public List<Prioridad> Prioridad { get; set; } = new List<Prioridad>();
        public async Task<JsonResult> OnGetUsuariosFiltradosAsync(string division)
        {
            var usuarios = await _context.GetUsuariosFiltradosAsync(int.Parse(division));
            return new JsonResult(usuarios);
        }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            Usuarios = await _context.GetUsuariosAsync();
            if (id == null)
            {
                return NotFound();
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
            // Guardar el valor de ID_Tarea_Fk en la sesión
            HttpContext.Session.SetInt32("ID_Tarea_Fk", PedidoTrabajo.ID_Tarea_Fk);
            HttpContext.Session.SetString("CorreoUsuario", PedidoTrabajo.Correo);
            HttpContext.Session.SetString("motivo", motivo);
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
        public async Task<IActionResult> OnPostSuspenderEstadoAsync(int OrdenTrabajoId, DateTime fechaEstimadaFin)
        {
            string fechaComoString = fechaEstimadaFin.ToString("dd/MM/yyyy");
            HttpContext.Session.SetString("FechaEstimadaFin", fechaComoString);
            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[SuspenderPedidoTrabajo] @p0, @p1", OrdenTrabajoId, fechaEstimadaFin);
            return RedirectToPage("./Index");
        }
        public async Task<IActionResult> OnPostCancelarEstadoAsync(int OrdenTrabajoId)
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[CancelarPedidoTrabajo] @p0", OrdenTrabajoId);
            return RedirectToPage("./Index");
        }
        public async Task<IActionResult> OnPostPendienteEstadoAsync(int OrdenTrabajoId)
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[PendientePedidoTrabajo] @p0", OrdenTrabajoId);
            return RedirectToPage("./Index");
        }
        

    }
}

