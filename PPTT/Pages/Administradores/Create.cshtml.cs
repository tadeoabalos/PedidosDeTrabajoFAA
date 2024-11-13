using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PPTT.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net.Mail;
using System.Net;
using System.Configuration;
using System.Data.SqlClient;

namespace PPTT.Pages.Administradores
{
    public class CreateModel : PageModel
    {
        private readonly PPTT.Data.DBPPTTContext _context;
        private readonly IConfiguration _configuration;
        public CreateModel(PPTT.Data.DBPPTTContext context, IConfiguration configuration)
        {
            _configuration = configuration; // Ahora se asigna el parámetro correctamente
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

            int DNI = Admin.DNI;
            HttpContext.Session.SetInt32("DNI", DNI);

            // Asegúrate de que esta sea una dirección de correo válida
            var EmailDestino = Admin.Correo; // Cambiado a una dirección válida
            var asunto = "Tu Nueva Contraseña";
            Random random = new Random();
            int randomNumber = random.Next(10000000, 100000000); // Límite inferior (inclusive) y superior (exclusive)
            var body = $"Su contraseña ahora es {randomNumber}";
            string randomString = randomNumber.ToString();
            byte[] bytesContraseña;
            bytesContraseña = ASCIIEncoding.ASCII.GetBytes(randomString);
            //lo hasheo
            byte[] hashContraseña;
            hashContraseña = MD5.HashData(bytesContraseña);
            Console.WriteLine(hashContraseña);
            bool isValid = await CrearContraStoredProcedure(DNI, hashContraseña);
            if (isValid)
            {
                SendMail(EmailDestino, asunto, body);
                Console.WriteLine("Contraseña actualizada correctamente.");
                int _roll = HttpContext.Session.GetInt32("UserRole") ?? 0;
                if (_roll == 3)
                {
                    return RedirectToPage("./Index");

                }
                else
                {
                    return RedirectToPage("./IndexAdminU");
                }
            }
            else
            {
                Console.WriteLine("Error al actualizar la contraseña.");
                return Page();
            }

    }

        private async Task<bool> CrearContraStoredProcedure(int DNI, byte[] hashContraseña)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionSQL");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("Crear_Password", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        // Pasar los parámetros al stored procedure
                        command.Parameters.AddWithValue("@DNI", DNI);
                        command.Parameters.AddWithValue("@Pass", hashContraseña);  // Enviar el hash en formato byte[]

                        // Ejecutar el stored procedure
                        await command.ExecuteNonQueryAsync();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
        private bool SendMail(string to, string asunto, string body)
        {
            string mailUser = _configuration["MailSettings:MailUser"];
            string mailPassword = _configuration["MailSettings:MailPassword"];
            string from = mailUser; // Asegúrate de que esta sea una dirección válida
            string displayName = "Soporte C.G Pedidos de Trabajo Fuerza Aerea Argentina";

            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(from, displayName);
                mail.To.Add(to);  // Asegúrate de que 'to' sea una dirección válida
                mail.Subject = asunto;
                mail.Body = body;
                mail.IsBodyHtml = true;  // false si es texto plano.
                SmtpClient client = new SmtpClient("10.0.8.19", 25); // mailserver.condor.faa
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(from, mailPassword);
                client.EnableSsl = false; // Ver si el servidor de correo maneja cifrado o poner falso si no lo maneja.
                client.Send(mail);
                return true;
            }
            catch (SmtpException ex)
            {
                // Manejo de errores con Console.WriteLine o cualquier otro método de registro.
                Console.WriteLine($"ERROR SMTP: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                // Manejo de errores generales.
                Console.WriteLine($"ERROR GENERAL: {ex.Message}");
                return false;
            }
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
                 Divisions = await _context.GetDosDivisionesPorUsuarioAsync(_Id_Division, _Id_Division2);
                 return Page();                
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
