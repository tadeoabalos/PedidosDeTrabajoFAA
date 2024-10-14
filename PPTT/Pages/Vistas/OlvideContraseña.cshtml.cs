using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using static PPTT.Models.Admin;
using System.Configuration;
using System.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;

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
        public void OnGet()
        {

        }
        public async Task<IActionResult> OnPostAsync()
        {
            // Asegúrate de que esta sea una dirección de correo válida
            var EmailDestino = Request.Form["Correo"]; // Cambiado a una dirección válida
            var asunto = "Cambio de Contraseña";
            Random random = new Random();
            int randomNumber = random.Next(10000000, 100000000); // Límite inferior (inclusive) y superior (exclusive)
            var body = $"Su contraseña ahora es {randomNumber}";
            bool result = SendMail(EmailDestino, asunto, body);
            string randomString = randomNumber.ToString();
            byte[] bytesContraseña;
            bytesContraseña = ASCIIEncoding.ASCII.GetBytes(randomString);
            //lo hasheo
            byte[] hashContraseña;
            hashContraseña = MD5.HashData(bytesContraseña);
            Console.WriteLine(hashContraseña);
            string sDNI = Request.Form["DNI"];
            int.TryParse(sDNI, out int DNI);
            string NumeroDeControl = Request.Form["Numero_De_Control"];
            bool isValid = await EjecutarValidarStoredProcedure(DNI, hashContraseña);
            if (result)
            {
                ViewData["Message"] = "Correo enviado correctamente.";
            }
            else
            {
                ViewData["Message"] = "Hubo un problema enviando el correo.";
            }
            return RedirectToPage("/Vistas/IngresoPersonal");
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
        private async Task<bool> EjecutarValidarStoredProcedure(int dni, byte[] password)
        {
            string? connectionString = _configuration.GetConnectionString("ConnectionSQL");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("Crear_Password", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        //ejecuto el stored procedure de login con estos valores
                        command.Parameters.AddWithValue("@DNI", dni);
                        command.Parameters.AddWithValue("@Pass", password);
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return true ;
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