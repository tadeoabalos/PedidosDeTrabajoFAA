using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace PPTT.Pages.Vistas
{
    public class OrdenModel : PageModel
    {
        [BindProperty]
        public required string NumeroDeOficina { get; set; }

        [BindProperty]
        public required string ColorDeOficina { get; set; }

        [BindProperty]
        public required string PisoOficina { get; set; }

        [BindProperty]
        public required string DivisionDeMantenimiento { get; set; }

        [BindProperty]
        public required string ServicioRequerido { get; set; }

        [BindProperty]
        public required string DetalleServicio { get; set; }

        [BindProperty]
        public required string Observacion { get; set; }

       public class ApplicationDbContext : DbContext
        {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : base(options)
            {
            }

            public DbSet<Orden> Ordenes { get; set; }  // Nota el plural para la convención

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                // Configurar la tabla 'orden_de_trabajo' si es necesario
                modelBuilder.Entity<Orden>().ToTable("orden_de_trabajo");
            }
        }

        public void OnGet()
        {

        }

        public class ordenModel : PageModel
        {
            private readonly ApplicationDbContext _context;

            public ordenModel(ApplicationDbContext context)
            {
                _context = context;
            }

            [BindProperty]
            public string NumeroDeOficina { get; set; }

            [BindProperty]
            public string ColorDeOficina { get; set; }

            [BindProperty]
            public string PisoOficina { get; set; }

            [BindProperty]
            public string DivisionDeMantenimiento { get; set; }

            [BindProperty]
            public string ServicioRequerido { get; set; }

            [BindProperty]
            public string DetalleServicio { get; set; }

            [BindProperty]
            public string Observacion { get; set; }

            public void OnGet()
            {
            }

            public async Task<IActionResult> OnPostAsync()
            {
                if (!ModelState.IsValid)
                {
                    return Page();
                }

                var orden = new Orden
                {
                    NumeroDeOficina = NumeroDeOficina,
                    ColorDeOficina = ColorDeOficina,
                    PisoOficina = PisoOficina,
                    DivisionDeMantenimiento = DivisionDeMantenimiento,
                    ServicioRequerido = ServicioRequerido,
                    DetalleServicio = DetalleServicio,
                    Observacion = Observacion
                };

                _context.Ordenes.Add(orden);
                await _context.SaveChangesAsync();

                return RedirectToPage("Success"); // Redirige a una página de éxito o confirmación
            }
        }

    }
    public class Orden
    {
        [BindProperty]
        public required string NumeroDeOficina { get; set; }

        [BindProperty]
        public required string ColorDeOficina { get; set; }

        [BindProperty]
        public required string PisoOficina { get; set; }

        [BindProperty]
        public required string DivisionDeMantenimiento { get; set; }

        [BindProperty]
        public required string ServicioRequerido { get; set; }

        [BindProperty]
        public required string DetalleServicio { get; set; }

        [BindProperty]
        public required string Observacion { get; set; }

    }
}


