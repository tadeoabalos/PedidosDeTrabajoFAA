using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PPTT.Models;
using System.Data;

namespace PPTT.Pages.Administradores
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly PPTT.Data.DBPPTTContext _context;       
        private IConfiguration? configuration;
        public IndexModel(PPTT.Data.DBPPTTContext context)
        {
            _context = context;
            _configuration = configuration; // Asegúrate de que esto esté presente
        }
        public List<ServicioModel> Items { get; set; } = new List<ServicioModel>();
        public List<string> Divisions { get; set; } = new List<string>(); // Asegúrate de que esta propiedad existe
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
                Admin = await _context.Usuario.ToListAsync();
                return Page();
            }
        }


    }
}
