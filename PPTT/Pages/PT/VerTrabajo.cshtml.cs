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
        public List<PTUsuario> PedidoTrabajo { get; set; } = new List<PTUsuario>();


        [BindProperty]
        public List<Admin> Usuarios { get; set; } = new List<Admin>();

        public List<Prioridad> Prioridad { get; set; } = new List<Prioridad>();


        public async Task OnGetAsync()
        {
            PedidoTrabajo = await _context.PTUsuario
                .Include(pt => pt.Organismo)
                .Include(pt => pt.Tarea)
                .Include(pt => pt.Estado)
                .Include(pt => pt.Prioridad)
                .Include(pt => pt.Dependencia_Interna)
                .Include(pt => pt.Grado)
                .ToListAsync();
        }



    }

}