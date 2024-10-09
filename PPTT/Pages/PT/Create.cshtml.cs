using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PPTT.Data;
using PPTT.Models;

namespace PPTT.Pages.PT
{
    public class CreatePPTT : PageModel
    {
        private readonly PPTT.Data.DBPPTTContext _context;
        public DbSet<PTUsuario> PTUsuarios { get; set; }
        public CreatePPTT(PPTT.Data.DBPPTTContext context)
        {
            _context = context;

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

            // Asigna valores automáticos a las propiedades de PedidoTrabajo
            PedidoTrabajo.ID_Estado_Fk = 1; // Automático
            PedidoTrabajo.Fecha_Subida = DateTime.Now; // Automático
            PedidoTrabajo.IP_Solicitante = HttpContext.Connection.RemoteIpAddress?.ToString();
            PedidoTrabajo.Prioridad = 1;

            // Manejo del valor nullable
            var tarea = PedidoTrabajo.ID_Tarea_Fk ?? 0; // Asigna 0 si ID_Tarea_Fk es null
            HttpContext.Session.SetInt32("tarea", tarea);

            try
            {
                // Agregar el nuevo PedidoTrabajo al contexto
                await _context.PTUsuarios.AddAsync(PedidoTrabajo);
                await _context.SaveChangesAsync(); // Guarda los cambios

                // Ejecutar el stored procedure Diferentes_IDs
                return RedirectToPage("./SubirID");
            }
            catch (Exception ex)
            {
                // Maneja el error (puedes loguearlo o mostrar un mensaje)
                ModelState.AddModelError(string.Empty, $"Error: {ex.Message}");
                return Page();
            }
        }


        public async Task<IActionResult> OnGetAsync()
        {
            Grados = await _context.GetGradosAsync();
            Servicios = await _context.GetServiciosSinFiltrarAsync();
            Organismos = await _context.GetOrganismoAsync();
            return Page();
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
