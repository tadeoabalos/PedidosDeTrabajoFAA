using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PPTT.Models;
using System.Net.Mail;
using System.Net;
using static PPTT.Models.Admin;
using Humanizer;
using System.Data;
using System.Data.SqlClient;


namespace PPTT.Pages.PT
{
    public class DetailsPPTT : PageModel
    {
        private readonly PPTT.Data.DBPPTTContext _context;
        private readonly IConfiguration _configuration;
        private string _correo;

        public DetailsPPTT(PPTT.Data.DBPPTTContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public List<Admin> Usuarios { get; set; } = new List<Admin>();
        public PTUsuario? PedidoTrabajo { get; set; } = default!;
        public List<Estado> Estado { get; set; } = default!;
        public List<Prioridad> Prioridad { get; set; } = new List<Prioridad>();
        public int PrioridadId;
        public List<Motivo> Motivos { get; set; } = new List<Motivo>(); 

        public void RetornaMotivo(int IdPt, int IdEstado)
        {           
            Motivos = _context.Motivo
                .FromSqlRaw("EXEC [dbo].[RetornaMotivo] @p0, @p1", IdPt, IdEstado)
                .ToList(); 
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            int _rol = HttpContext.Session.GetInt32("UserRole") ?? 0;
            if (_rol > 1)
            {
                if (id == null)
                {
                    return NotFound();
                }

                // Obtener los usuarios filtrados
                try
                {
                    Usuarios = await _context.GetUsuariosFiltradosByOrdenAsync(id);
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

                    // Verificar si se encontró el pedido de trabajo
                    if (PedidoTrabajo == null)
                    {
                        return NotFound();
                    }

                    // Lógica de envío de correo
                    int datos = HttpContext.Session.GetInt32("datoss") ?? 0;
                    if (datos == 1)
                    {
                        await SendStatusEmail(PedidoTrabajo);
                        HttpContext.Session.SetInt32("datoss", 0);
                        int rol = HttpContext.Session.GetInt32("UserRole") ?? 0;
                        if (rol == 3)
                        {
                            return RedirectToPage("./Index");
                        }
                        else
                        {
                            return RedirectToPage("./IndexAdmin");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Manejo de errores
                    Console.WriteLine($"ERROR: {ex.Message}");
                    return StatusCode(500, "Error interno del servidor.");
                }

                RetornaMotivo(PedidoTrabajo.ID_Orden_Trabajo_Pk, PedidoTrabajo.ID_Estado_Fk);
                return Page();

            }
            else
            {
                return RedirectToPage("/Index");
            }
        }

        private async Task SendStatusEmail(PTUsuario pedidoTrabajo)
        {
            string asunto = "Cambio de estado de su pedido de trabajo";
            string body = "";
            DateTime fechaActual = DateTime.Now;

            // Formatear la fecha en formato dd/MM/yyyy
            string fechaFormateada = fechaActual.ToString("dd/MM/yyyy");
            Console.WriteLine(pedidoTrabajo.ID_Estado_Fk);
            switch (pedidoTrabajo.ID_Estado_Fk)
            {
                case 1002:
                    body = $"Su pedido de trabajo #{pedidoTrabajo.ID_Orden_Fk} ha cambiado de estado a pendiente y está a la espera de procesamiento.";
                    break;
                case 1003:
                    body = $"Su solicitud de trabajo #{pedidoTrabajo.ID_Orden_Fk} se encuentra en curso. Se ha asignado al personal correspondiente para proceder con la resolución de la solicitud.";
                    break;
                case 1004:
                    body = $"Su pedido de trabajo #{pedidoTrabajo.ID_Orden_Fk} ha sido completado satisfactoriamente el día {fechaFormateada}.";
                    break;
                case 1005:
                    string fechaEstimada = HttpContext.Session.GetString("FechaEstimadaFin") ?? "fecha no disponible";
                    string motivo = HttpContext.Session.GetString("motivo") ?? "motivo no disponible";
                    body = $"Su pedido de trabajo #{pedidoTrabajo.ID_Orden_Fk} ha sido suspendido debido a {motivo}. Las labores se reanudarán a partir de {fechaEstimada}.";
                    break;
                case 1006:
                    string motivoo = HttpContext.Session.GetString("motivo") ?? "motivo no disponible";
                    body = $"Su pedido de trabajo #{pedidoTrabajo.ID_Orden_Fk} ha sido cancelado. Motivo: {motivoo}.";
                    break;
                default:
                    return; // No se envía correo si no hay un estado válido
            }

            // Enviar el correo
            Console.WriteLine(pedidoTrabajo.Correo);
            SendMail(pedidoTrabajo.Correo, asunto, body);
        }

        public async Task<JsonResult> OnGetPrioridadesAsync()
        {
            var prioridades = await _context.GetPrioridadAsync();
            return new JsonResult(prioridades);
        }

        public async Task<IActionResult> OnPostFinalizarEstadoAsync(int OrdenTrabajoId)
        {
            int datos = HttpContext.Session.GetInt32("datoss") ?? 0;
            datos++;
            HttpContext.Session.SetInt32("datoss", datos);
            int idUsuario = HttpContext.Session.GetInt32("IDUsuario") ?? 0;
            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[FinalizarPedidoTrabajo] @p0", OrdenTrabajoId);
            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[AuditarPT] @p0, @p1, @p2", OrdenTrabajoId, 1004, idUsuario);
            return RedirectToPage("./MandarMailCambioEstado");
        }

        public async Task<IActionResult> OnPostSuspenderEstadoAsync(int OrdenTrabajoId, DateTime fechaEstimadaFin, string motSus)
        {
            int datos = HttpContext.Session.GetInt32("datoss") ?? 0;
            datos++;
            HttpContext.Session.SetInt32("datoss", datos);
            string fechaComoString = fechaEstimadaFin.ToString("dd/MM/yyyy");
            HttpContext.Session.SetString("FechaEstimadaFin", fechaComoString);
            HttpContext.Session.SetString("motivo", motSus);
            int idUsuario = HttpContext.Session.GetInt32("IDUsuario") ?? 0;
            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[SuspenderPedidoTrabajo] @p0, @p1, @p2", OrdenTrabajoId, fechaEstimadaFin, motSus);
            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[AuditarPT] @p0, @p1, @p2", OrdenTrabajoId, 1005, idUsuario);
            return RedirectToPage("./MandarMailCambioEstado");
        }

        public async Task<IActionResult> OnPostEnProcesoEstadoAsync(int OrdenTrabajoId, int IdUsuario)
        {
            if (IdUsuario == 0)
            {
                ModelState.AddModelError(string.Empty, "Debe seleccionar un usuario.");
                return Page();
            }
            Console.WriteLine(IdUsuario);
            EjecutarObtenerCorreoStoredProcedure(IdUsuario);
            string corrreo = HttpContext.Session.GetString("correous");
            string asunto = "Se le ha asignado un trabajo";
            string body = $"Se le ha encargado el trabajo #{OrdenTrabajoId} ingrese para ver los detalles";
            SendMail(corrreo, asunto, body);  // Envía el correo del primer resultado

            // Actualiza la sesión
            int datos = HttpContext.Session.GetInt32("datoss") ?? 0;
            datos++;
            HttpContext.Session.SetInt32("datoss", datos);
            int idUsuario = HttpContext.Session.GetInt32("IDUsuario") ?? 0;
            // Ejecuta el procedimiento para asignar el usuario a la orden de trabajo
            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[AsignarUsuarioAOrden] @p0, @p1", IdUsuario, OrdenTrabajoId);
            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[AuditarPT] @p0, @p1, @p2", OrdenTrabajoId, 1003, idUsuario);
            return RedirectToPage("./MandarMailCambioEstado");
        }


        private async Task<bool> EjecutarObtenerCorreoStoredProcedure(int IdUsuario)
        {
            string? connectionString = _configuration.GetConnectionString("ConnectionSQL");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("Tomar_Mail", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@IDUSUARIO", IdUsuario);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                _correo = reader.GetString(0);
                                HttpContext.Session.SetString("correous", _correo);
                                Console.WriteLine(_correo, "loagarre");
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


        public async Task<IActionResult> OnPostCancelarEstadoConMotivoAsync(int OrdenTrabajoId, string motCan)
        {
            int datos = HttpContext.Session.GetInt32("datoss") ?? 0;
            datos++;
            HttpContext.Session.SetInt32("datoss", datos);
            HttpContext.Session.SetString("motivo", motCan);
            int idUsuario = HttpContext.Session.GetInt32("IDUsuario") ?? 0;
            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[CancelarPedidoTrabajo] @p0, @p1", OrdenTrabajoId, motCan);
            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[AuditarPT] @p0, @p1, @p2", OrdenTrabajoId, 1006, idUsuario);
            return RedirectToPage("./MandarMailCambioEstado");
        }

        public async Task<IActionResult> OnPostSetPrioridadAsync(int OrdenTrabajoId, int PrioridadId)
        {
            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[SetPrioridad] @p0, @p1", OrdenTrabajoId, PrioridadId);
            int _rol = HttpContext.Session.GetInt32("UserRole") ?? 0;
            if (_rol == 3)
            {
                return RedirectToPage("./Index");
            }
            else
            {
                return RedirectToPage("./IndexAdmin");
            }
            
        }

        public async Task<IActionResult> OnPostPendienteEstadoAsync(int OrdenTrabajoId)
        {
            int datos = HttpContext.Session.GetInt32("datoss") ?? 0;
            datos++;
            HttpContext.Session.SetInt32("datoss", datos);
            int idUsuario = HttpContext.Session.GetInt32("IDUsuario") ?? 0;
            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[PendientePedidoTrabajo] @p0", OrdenTrabajoId);
            await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[AuditarPT] @p0, @p1, @p2", OrdenTrabajoId, 1002, idUsuario);
            return RedirectToPage("./MandarMailCambioEstado");
        }



        private bool SendMail(string to, string asunto, string body)
        {
            string mailUser = _configuration["MailSettings:MailUser"];
            string mailPassword = _configuration["MailSettings:MailPassword"];
            string from = mailUser;
            string displayName = "Soporte C.G Pedidos de Trabajo Fuerza Aerea Argentina";
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(from, displayName);
                mail.To.Add(to);
                mail.Subject = asunto;
                mail.Body = body;
                mail.IsBodyHtml = true;
                SmtpClient client = new SmtpClient("10.0.8.19", 25);
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(from, mailPassword);
                client.EnableSsl = false;
                client.Send(mail);
                Console.WriteLine("true mail");
                return true;
            }
            catch (SmtpException ex)
            {
                Console.WriteLine("false mail");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("false mail");
                return false;
            }
        }
    }
}
