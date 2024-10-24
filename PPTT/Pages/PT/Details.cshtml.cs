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
using static PPTT.Models.Admin;

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

        public async Task<JsonResult> OnGetUsuariosFiltradosAsync(string division)
        {
            var usuarios = await _context.GetUsuariosFiltradosAsync(int.Parse(division));
            return new JsonResult(usuarios);
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            int _rol = HttpContext.Session.GetInt32("UserRole") ?? 0;
            HttpContext.Session.SetInt32("UserRole", _rol);
            if (_rol == 2)
        
        public async Task<IActionResult> OnGetAsync(int? id)
        {            
            if (id == null)
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

                HttpContext.Session.SetString("CorreoUsuario", PedidoTrabajo.Correo);
                var idEstadoSeleccionado = PedidoTrabajo.ID_Estado_Fk;
                HttpContext.Session.SetInt32("ID_Estado_Fk", idEstadoSeleccionado);

                Console.WriteLine(Estado);
                return Page();
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
                return RedirectToPage("/Vistas/MenuLog");
            }
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

        public async Task<IActionResult> OnPostSuspenderEstadoAsync(int OrdenTrabajoId, DateTime fechaEstimadaFin, string motivo)
        {
            string fechaComoString = fechaEstimadaFin.ToString("dd/MM/yyyy");
            HttpContext.Session.SetString("FechaEstimadaFin", fechaComoString);

            // Validar y almacenar el motivo en la sesión
            if (!string.IsNullOrEmpty(motivo))
            {
                HttpContext.Session.SetString("motivo", motivo);
            }
            else
            {
                throw new ArgumentNullException(nameof(motivo), "El motivo no puede ser nulo o vacío.");
            }

            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[SuspenderPedidoTrabajo] @p0, @p1", OrdenTrabajoId, fechaEstimadaFin);
            return RedirectToPage("./MandarMailCambioEstado");
        }
        public async Task<IActionResult> OnPostEnProcesoEstadoAsync(int OrdenTrabajoId, int IdUsuario)
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[AsignarUsuarioAOrden] @p0, @p1", IdUsuario ,OrdenTrabajoId);
            return RedirectToPage("./Index");
        }
        public async Task<IActionResult> OnPostCancelarEstadoAsync(int OrdenTrabajoId)
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[CancelarPedidoTrabajo] @p0", OrdenTrabajoId);
            return RedirectToPage("./MandarMailCambioEstado");
        }

        public async Task<IActionResult> OnPostPendienteEstadoAsync(int OrdenTrabajoId)
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[PendientePedidoTrabajo] @p0", OrdenTrabajoId);
            return RedirectToPage("./MandarMailCambioEstado");
        }
    }
}
