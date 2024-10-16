﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PPTT.Data;
using PPTT.Models;

namespace PPTT.Pages.Administradores
{
    public class IndexPPTT : PageModel
    {
        private readonly PPTT.Data.DBPPTTContext _context;        
        public IndexPPTT(PPTT.Data.DBPPTTContext context)
        {
            _context = context;
        }      
        public IList<PTUsuario> PedidoTrabajo { get;set; } = default!;
        [BindProperty]
        public List<Admin> Usuarios { get; set; } = new List<Admin>();
        public Orden_Asignada Orden_Asignada { get; set; }
        public PTUsuario PT { get; set; } = default!;
        public List<Prioridad> Prioridad { get; set; } = new List<Prioridad>();        
        public async Task OnGetAsync()
        {
            Prioridad = await _context.GetPrioridadAsync();
            PedidoTrabajo = await _context.PTUsuario
                .Include(pt => pt.Organismo)           
                .Include(pt => pt.Tarea)
                .Include(pt => pt.Estado)
                .Include(pt => pt.Prioridad)
                .Include(pt => pt.Dependencia_Interna)
                .Include(pt => pt.Grado)
                .ToListAsync();
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
        public async Task<JsonResult> OnGetPrioridadesAsync()
        {
            var prioridades = await _context.GetPrioridadAsync();
            return new JsonResult(prioridades);
        }
    }
}
