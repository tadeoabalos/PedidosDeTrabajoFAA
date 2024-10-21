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
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            Usuarios = await _context.GetUsuariosAsync();
            if (id == null)
            {
                return NotFound();
            }

            Estado = await _context.GetEstadosAsync();

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
    }
}
