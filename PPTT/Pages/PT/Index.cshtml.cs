using System;
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

        public async Task OnGetAsync()
        {
            PedidoTrabajo = await _context.PTUsuario.ToListAsync();
        }


    }
}
