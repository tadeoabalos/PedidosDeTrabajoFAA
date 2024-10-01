using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PPTT.Data;
using PPTT.Models;
using System.Collections.Generic;

namespace PPTT.Pages.Administradores
{
    public class IndexModel : PageModel
    {
        private readonly PPTT.Data.DBPPTTContext _context;
        private int datos;

        public IndexModel(PPTT.Data.DBPPTTContext context)
        {
            _context = context;
        }
        public List<ServicioModel> Items { get; set; } = new List<ServicioModel>();
        public IList<Admin> Admin { get;set; } = default!;
        public class ServicioModel
        {
            public int? ID_Servicio_Fk { get; set; }
        }
        public async Task<IActionResult> OnGetAsync()
        {
            int datos = HttpContext.Session.GetInt32("datos") ?? 0;
            HttpContext.Session.SetInt32("datos", datos);
            if (datos == 0)
            {
                datos = datos + 1;
                HttpContext.Session.SetInt32("datos", datos);
                Console.WriteLine(datos);
                return RedirectToPage("/Administradores/TraerServicio");
            }
            else  
            {
                Admin = await _context.usuario.ToListAsync();
                return Page();
            }
        }
    }
}
