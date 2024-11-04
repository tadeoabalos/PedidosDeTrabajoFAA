
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PPTT.Models;
namespace PPTT.Pages.Administradores
{
    public class EditModel : PageModel
    {
        private readonly PPTT.Data.DBPPTTContext _context;

        public EditModel(PPTT.Data.DBPPTTContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Admin Admin { get; set; } = default!;

        public List<Division> Divisions { get; set; } = new List<Division>();
        public List<SelectListItem> Roles { get; set; } = new List<SelectListItem>();

        public async Task<JsonResult> OnGetServiciosByDivisionAsync(string division)
        {
            if (int.TryParse(division, out int divisionId))
            {
                var servicios = await _context.GetServiciosAsync(divisionId);
                return new JsonResult(servicios.Select(s => new
                {
                    s.ID_Servicio_Pk,
                    s.Descripcion_Servicio
                }));
            }
            return new JsonResult(new List<object>());
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            Divisions = await _context.GetDivisionAsync();
            int _rol = HttpContext.Session.GetInt32("UserRole") ?? 0;
                Roles = Enum.GetValues(typeof(Admin.Rol))
    .Cast<Admin.Rol>()
    .Where(c => !(c == Admin.Rol.SuperAdministrador && _rol < 3)) // Filtra SuperAdministrador si _rol es menor que 3
    .Select(c => new SelectListItem
    {
        Value = ((int)c).ToString(),
        Text = c.ToString()
    }).ToList();

            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Usuario.FirstOrDefaultAsync(m => m.ID_Usuario_Pk == id);
            if (admin == null)
            {
                return NotFound();
            }

            Admin = admin;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var adminFromDb = await _context.Usuario.AsNoTracking().FirstOrDefaultAsync(m => m.ID_Usuario_Pk == Admin.ID_Usuario_Pk);
            if (adminFromDb == null)
            {
                return NotFound();
            }

            // Asignar las propiedades del objeto Admin basadas en el formulario
            adminFromDb.Nombre = Admin.Nombre;
            adminFromDb.Apellido = Admin.Apellido;
            adminFromDb.DNI = Admin.DNI;
            adminFromDb.Numero_Control = Admin.Numero_Control;
            adminFromDb.Correo = Admin.Correo;
            adminFromDb.ID_Rol_Fk = Admin.ID_Rol_Fk ?? 1; // Establecer ID_Rol_Fk con un valor predeterminado si es nulo
            adminFromDb.ID_Servicio_Fk = Admin.ID_Servicio_Fk ?? 1; // Asegúrate de que el ID_Servicio_Fk tenga un valor válido
            adminFromDb.ID_Division_Fk = Admin.ID_Division_Fk ?? 1; // Asegúrate de que el ID_Division_Fk tenga un valor válido

            // Actualizar el contexto
            _context.Usuario.Update(adminFromDb);

            try
            {
                // Llamar al stored procedure
                var parameters = new[]
                {
            new Microsoft.Data.SqlClient.SqlParameter("@IDUSUARIO", adminFromDb.ID_Usuario_Pk),
            new Microsoft.Data.SqlClient.SqlParameter("@IDROL", adminFromDb.ID_Rol_Fk),
            new Microsoft.Data.SqlClient.SqlParameter("@IDSERVICIO", adminFromDb.ID_Servicio_Fk),
            new Microsoft.Data.SqlClient.SqlParameter("@IDDIVISION", adminFromDb.ID_Division_Fk)
        };

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC Auditar @IDUSUARIO, @IDROL, @IDSERVICIO, @IDDIVISION",
                    parameters
                );

                await _context.SaveChangesAsync(); // Guardar los cambios en la base de datos
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdminExists(adminFromDb.ID_Usuario_Pk))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }







        private bool AdminExists(int id)
        {
            return _context.Usuario.Any(e => e.ID_Usuario_Pk == id);
        }
    }
}
