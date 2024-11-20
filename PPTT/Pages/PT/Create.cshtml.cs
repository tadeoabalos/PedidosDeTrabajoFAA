
using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PPTT.Models;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;
namespace PPTT.Pages.PT
{
    public class CreatePPTT : PageModel
    {
        private readonly PPTT.Data.DBPPTTContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CreatePPTT> _logger;

        public CreatePPTT(PPTT.Data.DBPPTTContext context, ILogger<CreatePPTT> logger, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;

            ColoresOficina = Enum.GetValues(typeof(ColorOficina))
                           .Cast<ColorOficina>()
                           .Select(c => new SelectListItem
                           {
                               Value = c.ToString(),
                               Text = c.ToString()
                           }).ToList();
            PisosOficina = Enum.GetValues(typeof(PisoOficina))
                           .Cast<PisoOficina>()
                           .Select(p => new SelectListItem
                           {
                               Value = p.ToString(),
                               Text = GetEnumDisplayName(p)
                           }).ToList();
        }

        [BindProperty]
        public PTUsuario PedidoTrabajo { get; set; } = default!;
        public List<Division> Division { get; set; } = new List<Division>();
        public List<Grado> Grados { get; set; } = new List<Grado>();
        public List<Organismo> Organismos { get; set; } = new List<Organismo>();
        public List<Servicio> Servicios { get; set; } = new List<Servicio>();
        public List<Tarea> Tareas { get; set; } = new List<Tarea>();
        public List<SelectListItem> ColoresOficina { get; set; }
        public List<SelectListItem> PisosOficina { get; set; }
        public string Mail { get; set; }
        public enum ColorOficina
        {
            AMARILLO,
            AZUL,
            BLANCO,
            ROSA,
            VERDE                        
        }
        public enum PisoOficina
        {
            [Display(Name = "Planta Baja (PB)")]
            PB,
            [Display(Name = "1.er Piso")]
            Primero,
            [Display(Name = "2.do Piso")]
            Segundo,
            [Display(Name = "3.er Piso")]
            Tercero,
            [Display(Name = "4.to Piso")]
            Cuarto,
            [Display(Name = "5.to Piso")]
            Quinto,
            [Display(Name = "6.to Piso")]
            Sexto,
            [Display(Name = "7.mo Piso")]
            Septimo,
            [Display(Name = "8.vo Piso")]
            Octavo,            
            [Display(Name = "9.no Piso")]
            Noveno,
            [Display(Name = "10.mo Piso")]
            Decimo
        }

        private string GetEnumDisplayName(Enum value)
        {
            var displayAttribute = value.GetType()
                .GetField(value.ToString())
                ?.GetCustomAttributes(typeof(DisplayAttribute), false)
                .FirstOrDefault() as DisplayAttribute;

            return displayAttribute?.Name ?? value.ToString();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        _logger.LogError($"Error en {state.Key}: {error.ErrorMessage}");
                    }
                }
                return RedirectToPage("../Index");
            }
            PedidoTrabajo.Fecha_Subida = DateTime.Now;
            PedidoTrabajo.IP_Solicitante = HttpContext.Connection.RemoteIpAddress?.ToString();
            PedidoTrabajo.ID_Prioridad_Fk = 1;

            //  Almacena el valor retornado del stored procedure en la propiedad ID_Orden_Fk
             PedidoTrabajo.ID_Orden_Fk = await EjecutarDiferentesIDs(PedidoTrabajo.ID_Tarea_Fk);
             //PedidoTrabajo.ID_Orden_Fk = 1;
            _context.PTUsuario.Add(PedidoTrabajo);

            await _context.SaveChangesAsync();
            string mail = PedidoTrabajo.Correo;
            string asunto = "Recibimos su Solicitud de Trabajo";
            string body = $"Hemos recibido su solicitud de trabajo. El número de identificación asignado es #{PedidoTrabajo.ID_Orden_Fk}. Le mantendremos informado sobre cualquier actualización en el estado de su solicitud.";
            SendMail(mail, asunto, body);
            return RedirectToPage("../PT/postCreate");
        }

        private async Task<int> EjecutarDiferentesIDs(int idTarea)
        {
            string connectionString = _configuration.GetConnectionString("ConnectionSQL");
            int idOrden = 0; // Variable para almacenar el ID que retorna el SP

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (SqlCommand command = new SqlCommand("Diferentes_IDs", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@ID_Tarea", idTarea));

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                idOrden = reader.GetInt32(0); // Asigna el ID retornado
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al ejecutar Diferentes_IDs: {ex.Message}");
            }

            return idOrden;
        }

        public async Task<IActionResult> OnGetAsync()
        {
                Division = await _context.GetDivisionAsync();
                Grados = await _context.GetGradosAsync();
                Organismos = await _context.GetOrganismoAsync();
                return Page();
        }
        public async Task<JsonResult> OnGetServiciosByDivisionAsync(string division)
        {
            var servicios = await _context.GetServiciosAsync(int.Parse(division));
            return new JsonResult(servicios);
        }
        public async Task<JsonResult> OnGetTareaByServicioAsync(string servicio)
        {
            var tareas = await _context.GetTareasFiltradasAsync(int.Parse(servicio));
            return new JsonResult(tareas);
        }
        public async Task<JsonResult> OnGetDependenciasByOrganismoAsync(string organismo)
        {
            var dependencias = await _context.GetDependenciasFiltradasAsync(int.Parse(organismo));
            return new JsonResult(dependencias);
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
                return true;
            }
            catch (SmtpException ex)
            {
                Console.WriteLine($"ERROR SMTP: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR GENERAL: {ex.Message}");
                return false;
            }
        }
    }
}
