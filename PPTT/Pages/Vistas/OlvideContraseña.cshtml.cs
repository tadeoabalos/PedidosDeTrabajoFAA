using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using System.Data.SqlClient;


namespace PPTT.Pages.Vistas
{
    public class OlvideContraseñaModel : PageModel
    {
        private readonly IConfiguration _configuration;

        // Constructor para inyectar IConfiguration
        public OlvideContraseñaModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult OnGet()
        {
            //si el rol es mayor a 0 significa que esta loggeado asi que hago que no pueda volver a la pagina de loggeo y sea redireccionado a una pagina para cerrar sesion
            int _rol = HttpContext.Session.GetInt32("UserRole") ?? 0;
            HttpContext.Session.SetInt32("UserRole", _rol);
            Console.WriteLine(_rol);
            HttpContext.Session.SetInt32("UserRole", _rol);
            //lo llevo a una pagina hecha para cambiar su contraseña para sacar la predeterminada
            if (_rol == 0)
            {
                return Page();
            }
            //decido a que menu lo mando, si al normal o al de admin
            else if (_rol == 1)
            {
                return RedirectToPage("/Vistas/MenuLog");
            }
            else if (_rol == 2)
            {
                return RedirectToPage("/Vistas/IndexAdmin");
            }
            else if (_rol == 3)
            {
                return RedirectToPage("/Vistas/IndexLogueado");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Rol no reconocido.");
                return Page();
            }

        }
        public async Task<IActionResult> OnPostAsync()
        {
            // Asegúrate de que esta sea una dirección de correo válida
            var EmailDestino = Request.Form["Correo"]; // Cambiado a una dirección válida
            var asunto = "Cambio de Contraseña";
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
            string sDNI = Request.Form["DNI"];
            int.TryParse(sDNI, out int DNI);
            bool isValid = await EjecutarValidarStoredProcedure(DNI, hashContraseña, EmailDestino);
            if (isValid)
            {
                SendMail(EmailDestino, asunto, body);
                return RedirectToPage("/Vistas/IngresoPersonal");
            }
            else
            {

                return Page();
            }
        }

        private bool SendMail(string to, string asunto, string body)
        {
            string mailUser = _configuration["MailSettings:MailUser"];
            string mailPassword = _configuration["MailSettings:MailPassword"];
            string from = mailUser; // Asegúrate de que esta sea una dirección válida
            string displayName = "Soporte Turnos Web Fuerza Aerea Argentina";

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
        private async Task<bool> EjecutarValidarStoredProcedure(int dni, byte[] password, string correo)
        {
            string? connectionString = _configuration.GetConnectionString("ConnectionSQL");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("Olvide_Password", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        //ejecuto el stored procedure de login con estos valores
                        command.Parameters.AddWithValue("@DNI", dni);
                        command.Parameters.AddWithValue("@Pass", password);
                        command.Parameters.AddWithValue("@Correo", correo);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
    }
}