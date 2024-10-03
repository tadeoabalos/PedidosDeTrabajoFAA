using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PPTT.Data;
using PPTT.Models;

namespace PPTT.Pages.PT
{
    public class CreatePPTT : PageModel
    {
        private readonly PPTT.Data.DBPPTTContext _context;

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
            [Display(Name = "1.er piso")]
            Primero,
            [Display(Name = "2.do piso")]
            Segundo,
            [Display(Name = "3.er piso")]
            Tercero,
            [Display(Name = "4.to piso")]
            Cuarto,
            [Display(Name = "5.to piso")]
            Quinto,
            [Display(Name = "6.to piso")]
            Sexto,
            [Display(Name = "7.mo piso")]
            Septimo,
            [Display(Name = "8.vo piso")]
            Octavo,            
            [Display(Name = "9.no piso")]
            Noveno,
            [Display(Name = "10.mo piso")]
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
                        
            PedidoTrabajo.ID_Organismo_Fk = 1; // Manual
            PedidoTrabajo.ID_Dependencia_Interna_Fk = 1; // Manual
            PedidoTrabajo.ID_Estado_Fk = 1; // Automático
            PedidoTrabajo.Fecha_Subida = DateTime.Now; // Automático
            PedidoTrabajo.IP_Solicitante = "181.285.984"; // Función Automática
            PedidoTrabajo.Prioridad = 1;
            _context.PTUsuario.Add(PedidoTrabajo); 

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Grados = await _context.GetGradosAsync();
            Servicios = await _context.GetServiciosSinFiltrarAsync();
            return Page();
        }

        public async Task<JsonResult> OnGetTareaByServicioAsync(string servicio)
        {
            var tareas = await _context.GetTareasFiltradasAsync(int.Parse(servicio));
            return new JsonResult(tareas);
        }

    }
}
