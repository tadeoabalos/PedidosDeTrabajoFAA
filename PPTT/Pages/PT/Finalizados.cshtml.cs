using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PPTT.Data;
using PPTT.Models;
using X.PagedList;
using PPTT.Models;
using X.PagedList.Extensions;

namespace PPTT.Pages.Administradores
{
    public class IndexPTFin : PageModel
    {
        private readonly PPTT.Data.DBPPTTContext _context;        
        public IndexPTFin(PPTT.Data.DBPPTTContext context)
        {
            _context = context;
        }
        public IPagedList<PTUsuario> PedidoTrabajo { get; set; } = default!;
        [BindProperty]
        public List<Admin> Usuarios { get; set; } = new List<Admin>();
        public Orden_Asignada_Usuario Orden_Asignada { get; set; }
        public PTUsuario PT { get; set; } = default!;
        public List<Prioridad> Prioridad { get; set; } = new List<Prioridad>();
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int PageNumber { get; set; } = 1;

        public async Task OnGetAsync(int? pageNumber)
        {
            int _rol = HttpContext.Session.GetInt32("UserRole") ?? 0;
            if (_rol > 1)
            {
                Prioridad = await _context.GetPrioridadAsync();

                // Si pageNumber no es proporcionado, usamos el valor predeterminado (1)
                PageNumber = pageNumber ?? 1;

                var pedidosQuery = _context.PTUsuario
                    .Include(pt => pt.Organismo)
                    .Include(pt => pt.Tarea)
                    .Include(pt => pt.Estado)
                    .Include(pt => pt.Prioridad)
                    .Include(pt => pt.Dependencia_Interna)
                    .Include(pt => pt.Grado)
                    .Include(pt => pt.Division)
                    .AsNoTracking()
                    .Where(pt => pt.ID_Estado_Fk == 1004 || pt.ID_Estado_Fk == 1006);

                // Paginación
                PedidoTrabajo = pedidosQuery.ToPagedList(PageNumber, 8);
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
        public async Task<JsonResult> OnGetUsuarioPorPtAsync(string PT) 
        {
            var usuario = await _context.GetUsuarioPorPtAsync(int.Parse(PT));
            return new JsonResult(usuario);
        }
        public async Task<IActionResult> OnPostAsignarUsuarioAsync(int UsuarioId, int OrdenTrabajoId)
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC AsignarUsuarioAOrden @p0, @p1", UsuarioId, OrdenTrabajoId);
            return RedirectToPage("./Index");
        }
        public async Task<IActionResult> OnPostSetPrioridadAsync(int OrdenTrabajoId, int PrioridadId)
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[SetPrioridad] @p0, @p1", OrdenTrabajoId, PrioridadId);
            return RedirectToPage("./Index");
        }

        public async Task<JsonResult> OnGetPrioridadesAsync()
        {
            var prioridades = await _context.GetPrioridadAsync();
            return new JsonResult(prioridades);
        }       
    }
}
