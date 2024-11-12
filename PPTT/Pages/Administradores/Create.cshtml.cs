using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PPTT.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;


namespace PPTT.Pages.Administradores
{
    public class CreateModel : PageModel
    {
        private readonly PPTT.Data.DBPPTTContext _context;

        public CreateModel(PPTT.Data.DBPPTTContext context)
        {
            _context = context;
            Admin = new Admin();
        }

        [BindProperty]
        public Admin Admin { get; set; }
        public List<Division> Divisions { get; set; } = new List<Division>();
        public List<Division> Div { get; set; } = new List<Division>();
        public List<Servicio> Servicios { get; set; } = new List<Servicio>();
        public int DNI { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Verificar si el DNI, Correo o NumeroDeControl ya existen en la base de datos
            var usuarioExistente = await _context.Usuario.FirstOrDefaultAsync(u =>
                u.DNI == Admin.DNI ||
                u.Correo == Admin.Correo ||
                u.Numero_Control == Admin.Numero_Control);

            if (usuarioExistente != null)
            {
                if (usuarioExistente.DNI == Admin.DNI)
                {
                    ModelState.AddModelError(string.Empty, "El DNI ya existe en la base de datos.");
                }
                if (usuarioExistente.Correo == Admin.Correo)
                {
                    ModelState.AddModelError(string.Empty, "El correo ya existe en la base de datos.");
                }
                if (usuarioExistente.Numero_Control == Admin.Numero_Control)
                {
                    ModelState.AddModelError(string.Empty, "El número de control ya existe en la base de datos.");
                }
                return Page();
            }

            // Configuración de las propiedades del nuevo usuario
            Admin.ID_Rol_Fk = 1;
            Admin.Fecha_Baja = new DateTime(1, 1, 1);
            Admin.Fecha_Alta = DateTime.Now;

            _context.Usuario.Add(Admin);
            await _context.SaveChangesAsync();

            DNI = Admin.DNI;
            HttpContext.Session.SetInt32("DNI", DNI);

            string numeroStr = DNI.ToString();
            string numeroInvertido = new string(numeroStr.Reverse().ToArray());

            byte[] bytesContraseña = Encoding.ASCII.GetBytes(numeroInvertido);
            byte[] hashContraseña = MD5.HashData(bytesContraseña);
            HttpContext.Session.Set("hashContraseña", hashContraseña);

            return RedirectToPage("/Administradores/SubirPass");
        }

        public async Task<IActionResult> OnGetAsync()
        {
            int _rol = HttpContext.Session.GetInt32("UserRole") ?? 0;
            int _Id_Division = HttpContext.Session.GetInt32("Division") ?? 0;
            int _Id_Division2 = HttpContext.Session.GetInt32("Division2") ?? 0;

            Divisions = await _context.Divisions.ToListAsync();
            if (_rol < 2)
            {
                return RedirectToPage("/Index");
            }
            else if (_rol == 3)
            {
                Divisions = await _context.GetDivisionAsync();
                return Page();
            }
            else if (_rol == 2)
            {
                if (_Id_Division2 == 0)
                {
                    Divisions = await _context.GetDivisionesPorUsuarioAsync(_Id_Division);
                    return Page();
                }
                else {
                    Divisions = await _context.GetDosDivisionesPorUsuarioAsync(_Id_Division, _Id_Division2);
                    return Page();
                }                
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Rol no reconocido.");
                return Page();
            }
        }

        public async Task<JsonResult> OnGetServiciosByDivisionAsync(string division)
        {
            var servicios = await _context.GetServiciosAsync(int.Parse(division));
            return new JsonResult(servicios);
        }        
    }
}
