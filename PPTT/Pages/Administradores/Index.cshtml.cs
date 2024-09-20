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
    public class IndexModel : PageModel
    {
        private readonly PPTT.Data.DBPPTTContext _context;

        public IndexModel(PPTT.Data.DBPPTTContext context)
        {
            _context = context;
        }

        public IList<Admin> Admin { get;set; } = default!;        

        public async Task OnGetAsync()
        {
            Admin = await _context.Usuarios.ToListAsync();            
        }
    }
}
