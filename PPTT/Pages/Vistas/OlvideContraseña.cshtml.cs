using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Mail;
using System.Net;

namespace PPTT.Pages.Vistas
{
    public class OlvideContraseñaModel : PageModel
    {
        public void OnGet()
        {
            // Configura los detalles del correo
            var fromAddress = new MailAddress("u.tinto@faa.mil.arg", "Uriel");
            var toAddress = new MailAddress("t.abalos@faa.mil.arg", "Tadeo");
            const string fromPassword = "584621584621"; // La contraseña de tu correo
            const string subject = "Asunto del correo";
            const string body = "Cuerpo del correo";

            // Crea el objeto SmtpClient
            var smtp = new SmtpClient
            {
                Host = "smtp.example.com", // Cambia esto por el servidor SMTP que estás usando
                Port = 587, // Puerto común para SMTP
                EnableSsl = true, // Habilita SSL
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            // Crea el mensaje
            var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            };

            // Envía el correo
            smtp.Send(message);
        }
    }
}

