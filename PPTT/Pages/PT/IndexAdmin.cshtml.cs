using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PPTT.Data;
using PPTT.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataContext = PPTT.Data.DBPPTTContext;
using ModelsContext = PPTT.Models.DBPPTTContext;

namespace PPTT.Pages.Administradores
{
    public class IndexAdminPPTT : PageModel
    {
        private readonly DataContext _context;

        public IndexAdminPPTT(DataContext context)
        {
            _context = context;
        }

        public PaginatedListAdmin<PTUsuario> PedidoTrabajo { get; set; } = default!;

        [BindProperty]
        public List<Admin> Usuarios { get; set; } = new List<Admin>();

        public List<Prioridad> Prioridad { get; set; } = new List<Prioridad>();
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        public async Task OnGetAsync(int? PageIndex, DateTime? fechaInicio, DateTime? fechaFin)
        {
            int pageSize = 12;
            var pedidosQuery = _context.PTUsuario
                .Include(pt => pt.Organismo)
                .Include(pt => pt.Tarea)
                .Include(pt => pt.Estado)
                .Include(pt => pt.Prioridad)
                .Include(pt => pt.Dependencia_Interna)
                .Include(pt => pt.Grado)
                .AsQueryable();

            // Filtros de fecha
            if (fechaInicio.HasValue && fechaFin.HasValue)
            {
                var fechaFinFinal = fechaFin.Value.Date.AddDays(1).AddTicks(-1);
                pedidosQuery = pedidosQuery.Where(pt => pt.Fecha_Subida >= fechaInicio && pt.Fecha_Subida <= fechaFinFinal);
            }

            // Paginación
            PedidoTrabajo = await PaginatedListAdmin<PTUsuario>.CreateAsync(pedidosQuery, PageIndex ?? 1, pageSize);
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
            return RedirectToPage("/PT/IndexAdmin");
        }
        public async Task<IActionResult> OnPostSetPrioridadAsync(int OrdenTrabajoId, int PrioridadId)
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[SetPrioridad] @p0, @p1", OrdenTrabajoId, PrioridadId);
            return RedirectToPage("/PT/IndexAdmin");
        }

        public async Task<JsonResult> OnGetPrioridadesAsync()
        {
            var prioridades = await _context.GetPrioridadAsync();
            return new JsonResult(prioridades);
        }
    }

    public class PaginatedListAdmin<T> : List<T>
    {
        public int PageIndex { get; private set; } // Cambiado a PageIndex
        public int TotalPages { get; private set; }

        public PaginatedListAdmin(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex; // Cambiado a PageIndex
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            this.AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public static async Task<PaginatedListAdmin<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedListAdmin<T>(items, count, pageIndex, pageSize); // Cambiado a pageIndex
        }
    }

}
