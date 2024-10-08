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

        }
        //private bool SendMail(string to, string asunto, string body)
        //{
        //    String[] UsuarioMail = System.Configuration.ConfigurationManager.AppSettings["MailUser"].Split(';'); // "Usuario: twsigeho;clave:1CABoca?  
        //    String from = UsuarioMail[0] + "@faa.mil.ar";
        //    String displayName = "Soporte Turnos Web Fuerza Aerea Argentina";

        //    try
        //    {
        //        MailMessage mail = new MailMessage();
        //        mail.From = new MailAddress(from, displayName);
        //        mail.To.Add(to);  // Todos los destinatarios, puede usarse copia oculta
        //        mail.Subject = asunto;
        //        mail.Body = body;
        //        mail.IsBodyHtml = true;  // false si es texto plano.
        //        SmtpClient client = new SmtpClient("10.0.8.19", 25);//mailserver.condor.faa
        //        client.UseDefaultCredentials = false;
        //        client.Credentials = new NetworkCredential(from, UsuarioMail[1]);
        //        client.EnableSsl = false; // Ver si el servidor de correo maneja cifrado o poner falso si no lo maneja.
        //        client.Send(mail);                                                                         //COLOCAR EL MAIL COMPLETO SIN EL @FAA.MIL.AR
        //        return true;
        //    }
        //    catch (SmtpException ex)
        //    {
        //        UC_Mensaje.ShowMensaje("ERROR", ex.Message, Shared_Controls_Application_Mesaje.TipoMensaje.Error);
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        UC_Mensaje.ShowMensaje("ERROR", ex.Message, Shared_Controls_Application_Mesaje.TipoMensaje.Error);
        //        return false;
        //    }
        }
    }

