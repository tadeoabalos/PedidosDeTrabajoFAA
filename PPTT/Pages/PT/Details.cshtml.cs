using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PPTT.Models;
using System.Net.Mail;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PPTT.Pages.PT
{
    public class DetailsPPTT : PageModel
    {
        private readonly PPTT.Data.DBPPTTContext _context;

        public DetailsPPTT(PPTT.Data.DBPPTTContext context)
        {
            _context = context;
        }
        private readonly IConfiguration _configuration;
        public DetailsPPTT(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [BindProperty]
public int ID_Estado_Fk { get; set; }

        public List<Admin> Usuarios { get; set; } = new List<Admin>();
        public PTUsuario? PedidoTrabajo { get; set; } = default!;
        public PTUsuario? MotivoSuspension { get; set; } = default!;
        public PTUsuario? IdUsuario { get; set; } = default!;
        public List<Estado> Estado { get; set; } = default!;
        public List<Prioridad> Prioridad { get; set; } = new List<Prioridad>();

        public async Task<JsonResult> OnGetUsuariosFiltradosAsync(string division)
        {
            var usuarios = await _context.GetUsuariosFiltradosAsync(int.Parse(division));
            return new JsonResult(usuarios);
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            Usuarios = await _context.GetUsuariosAsync();
            if (id == null)
            {
                return NotFound();
            }

            Estado = await _context.GetEstadosAsync();
            Prioridad = await _context.GetPrioridadAsync();
            PedidoTrabajo = await _context.PTUsuario
                .Include(pt => pt.Organismo)
                .Include(pt => pt.Tarea)
                .Include(pt => pt.Estado)
                .Include(pt => pt.Dependencia_Interna)
                .Include(pt => pt.Grado)
                .Include(pt => pt.Prioridad)
                .FirstOrDefaultAsync(m => m.ID_Orden_Trabajo_Pk == id);

            if (PedidoTrabajo == null)
            {
                return NotFound();
            }
                HttpContext.Session.SetInt32("ID_Estado_Fk", PedidoTrabajo.ID_Estado_Fk);
                HttpContext.Session.SetString("CorreoUsuario", PedidoTrabajo.Correo);
            int ida = PedidoTrabajo.ID_Orden_Trabajo_Pk;
            string idstring = ida.ToString();
            HttpContext.Session.SetString("ID", idstring);
            return Page();
        }

        public async Task<JsonResult> OnGetPrioridadesAsync()
        {
            var prioridades = await _context.GetPrioridadAsync();
            return new JsonResult(prioridades);
        }

        private async Task<PTUsuario?> GetPedidoTrabajoAsync(int ordenTrabajoId)
        {
            return await _context.PTUsuario
                .Include(pt => pt.Estado)
                .FirstOrDefaultAsync(pt => pt.ID_Orden_Trabajo_Pk == ordenTrabajoId);
        }

        public async Task<IActionResult> OnPostFinalizarEstadoAsync(int OrdenTrabajoId)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            int datos = HttpContext.Session.GetInt32("datos") ?? 0;
            datos = datos + 1;
            HttpContext.Session.SetInt32("datos", datos);
            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[FinalizarPedidoTrabajo] @p0", OrdenTrabajoId);
            return RedirectToPage("./MandarMailCambioEstado");
        }

        public async Task<IActionResult> OnPostSuspenderEstadoAsync(int OrdenTrabajoId, DateTime fechaEstimadaFin, string MotivoSuspension)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            int datos = HttpContext.Session.GetInt32("datos") ?? 0;
            datos = datos + 1;
            HttpContext.Session.SetInt32("datos", datos);
            string fechaComoString = fechaEstimadaFin.ToString("dd/MM/yyyy");
            HttpContext.Session.SetString("FechaEstimadaFin", fechaComoString);

            if (!string.IsNullOrEmpty(MotivoSuspension))
            {
                HttpContext.Session.SetString("motivo", MotivoSuspension);
            }
            else
            {
                throw new ArgumentNullException(nameof(MotivoSuspension), "El motivo no puede ser nulo o vacío.");
            }

            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[SuspenderPedidoTrabajo] @p0, @p1", OrdenTrabajoId, fechaEstimadaFin);
            return RedirectToPage("./MandarMailCambioEstado");
        }

        public async Task<IActionResult> OnPostCancelarEstadoAsync(int OrdenTrabajoId)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            int datos = HttpContext.Session.GetInt32("datos") ?? 0;
            datos = datos + 1;
            HttpContext.Session.SetInt32("datos", datos);
            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[CancelarPedidoTrabajo] @p0", OrdenTrabajoId);
            return RedirectToPage("./MandarMailCambioEstado");
        }

        public async Task<IActionResult> OnPostPendienteEstadoAsync(int OrdenTrabajoId)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            int datos = HttpContext.Session.GetInt32("datos") ?? 0;
            datos = datos + 1;
            HttpContext.Session.SetInt32("datos", datos);
            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[PendientePedidoTrabajo] @p0", OrdenTrabajoId);
            return RedirectToPage("./MandarMailCambioEstado");
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

    }
}
