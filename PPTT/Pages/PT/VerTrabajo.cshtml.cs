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


namespace PPTT.Pages.Vistas.VerTrabajo
{
    public class VerTrabajoPPTT : PageModel
    {
        private readonly DataContext _context;

        public VerTrabajoPPTT(DataContext context)
        {
            _context = context;
        }

        public PTUsuario PT { get; set; } = default!;
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
        }


    }

}