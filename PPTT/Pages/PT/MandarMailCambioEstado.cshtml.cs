using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Mail;
using System.Net;


namespace PPTT.Pages.PT
{
    public class MandarMailCambioEstadoModel : PageModel
    {
        private readonly IConfiguration _configuration;

        // Constructor para inyectar IConfiguration
        public MandarMailCambioEstadoModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public int? ID_Estado_Fk { get; set; }
        public string? FechaEstimada { get; set; }
        public string? CorreoUsuario { get; set; }
        public string? desestado { get; set; }
        public string? motivo { get; set; }
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
        public IActionResult OnGet()
        {
            int datos = HttpContext.Session.GetInt32("datos") ?? 0;
            HttpContext.Session.SetInt32("datos", datos);
            if (datos == 0)
            {
                return Page();
            }
            else if (datos == 1)
            {
                return RedirectToPage("/PT/Details");
            }
            else
            {
                ID_Estado_Fk = HttpContext.Session.GetInt32("ID_Estado_Fk");
                CorreoUsuario = HttpContext.Session.GetString("CorreoUsuario");
                FechaEstimada = HttpContext.Session.GetString("FechaEstimadaFin");
                motivo = HttpContext.Session.GetString("motivo");
                string asunto = "Cambio de estado de su pedido de trabajo";
                string body = "";
                Console.WriteLine(ID_Estado_Fk);
                Console.WriteLine(ID_Estado_Fk);

                Console.WriteLine(ID_Estado_Fk);

                Console.WriteLine(ID_Estado_Fk);

                Console.WriteLine(ID_Estado_Fk);

                Console.WriteLine(ID_Estado_Fk);

                Console.WriteLine(ID_Estado_Fk);

                Console.WriteLine(ID_Estado_Fk);

                Console.WriteLine(ID_Estado_Fk);

                Console.WriteLine(ID_Estado_Fk);

                Console.WriteLine(ID_Estado_Fk);


                if (ID_Estado_Fk == 1003)
                {
                    body = "Su pedido de trabajo está en proceso.";
                    SendMail(CorreoUsuario, asunto, body);
                }
                else if (ID_Estado_Fk == 1004)
                {
                    body = "Su pedido de trabajo ha sido finalizado.";
                    SendMail(CorreoUsuario, asunto, body);
                }
                else if (ID_Estado_Fk == 1005)
                {
                    body = $"Su pedido de trabajo ha sido suspendido. Fecha de inicio estimada: {FechaEstimada}.    Motivo: {motivo} ";
                    SendMail(CorreoUsuario, asunto, body);
                }
                else if (ID_Estado_Fk == 1006)
                {
                    body = $"Su pedido de trabajo ha sido cancelado.";
                    SendMail(CorreoUsuario, asunto, body);
                }
                else { }
                HttpContext.Session.SetInt32("datos", 0);
                return RedirectToPage("/PT/Index");
            }
        }
    }   
}
