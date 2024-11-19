using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PPTT.Models;
using X.PagedList.Extensions;
using X.PagedList;

namespace PPTT.Pages.Administradores
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly PPTT.Data.DBPPTTContext _context;
        public IPagedList<Admin> Admin { get; set; } = default!;
        public IndexModel(PPTT.Data.DBPPTTContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public List<ServicioModel> Items { get; set; } = new List<ServicioModel>();
        public List<string> Divisions { get; set; } = new List<string>();
        public int PageNumber { get; set; } = 1;
        
        [BindProperty(SupportsGet = true)]
        public string SearchQuery { get; set; }

        public class ServicioModel
        {
            public int? ID_Servicio_Fk { get; set; }
        }
        public DbSet<Division> Divisiones { get; set; }
        public async Task<IActionResult> OnGetAsync(int? pageNumber)
        {
            PageNumber = pageNumber ?? 1;
            int _rol = HttpContext.Session.GetInt32("UserRole") ?? 0;
            HttpContext.Session.SetInt32("UserRole", _rol);

            if (_rol < 2)
            {
                return RedirectToPage("/Index");
            }
            else if (_rol == 3)
            {
                int datos = HttpContext.Session.GetInt32("datos") ?? 0;
                HttpContext.Session.SetInt32("datos", datos);

                if (datos == 0)
                {
                    datos += 1;
                    HttpContext.Session.SetInt32("datos", datos);
                    return RedirectToPage("/Administradores/TraerServicio");
                }
                else
                {
                    var query = _context.Usuario.AsQueryable();
                    
                    query = query.Where(u => u.ID_Rol_Fk != 3); 

                    if (!string.IsNullOrEmpty(SearchQuery))
                    {
                        if (int.TryParse(SearchQuery, out int dniQuery))
                        {
                            query = query.Where(u => u.DNI == dniQuery);
                        }
                        else
                        {
                            query = query.Where(u =>
                                u.Nombre.Contains(SearchQuery) ||
                                u.Apellido.Contains(SearchQuery) ||
                                u.Correo.Contains(SearchQuery));
                        }
                    }                    
                    Admin = query.ToPagedList(PageNumber, 8);
                    return Page();
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Rol no reconocido.");
                return Page();
            }
        }
    }
}
