﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using PPTT.Data;
using PPTT.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Configuration;
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
        public List<SelectListItem> ColoresOficina { get; set; }
        public List<SelectListItem> PisosOficina { get; set; }
        public enum ColorOficina
        {
            AZUL,
            VERDE,
            MARRÓN,
            BLANCO,
            ROJO
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
                return Page();
            }

            PedidoTrabajo.Fecha_Subida = DateTime.Now;
            PedidoTrabajo.IP_Solicitante = HttpContext.Connection.RemoteIpAddress?.ToString();
            PedidoTrabajo.ID_Prioridad_Fk = 1;

            // Almacena el valor retornado del stored procedure en la propiedad ID_Orden_Fk
            PedidoTrabajo.ID_Orden_Fk = await EjecutarDiferentesIDs(PedidoTrabajo.ID_Tarea_Fk);

            _context.PTUsuario.Add(PedidoTrabajo);

            await _context.SaveChangesAsync();

            return RedirectToPage("/Index");
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
                                _logger.LogInformation($"ID Orden retornado: {idOrden}");
                                _logger.LogInformation($"ID Orden retornado: {idOrden}");
                                _logger.LogInformation($"ID Orden retornado: {idOrden}");
                                Console.WriteLine(idOrden);
                                Console.WriteLine(idOrden);
                                Console.WriteLine(idOrden);

                                Console.WriteLine(idOrden);
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

    }
}
