using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace PPTT.Models
{
    public class DivisionServicioViewModel
    {
        public DivisionServicioViewModel()
        {
            var DivisionMantenimiento = new SelectListGroup { Name = "División Mantenimiento de Instalaciones" };
            var DivisionInformatica = new SelectListGroup { Name = "División Comunicaciones e Informática" };

            Divisiones = new List<SelectListItem>
        {

            new SelectListItem
            {
                Value = "TEL",
                Text = "Telefonía",
                Group = DivisionInformatica
            },
            new SelectListItem
            {
                Value = "TAB",
                Text = "Tablero",
                Group = DivisionMantenimiento
            },
            new SelectListItem
            {
                Value = "ALB",
                Text = "Albañilería",
                Group = DivisionMantenimiento
            },
            new SelectListItem
            {
                Value = "TBQ",
                Text = "Tabiquería",
                Group = DivisionMantenimiento
            },
            new SelectListItem
            {
                Value = "CAR",
                Text = "Carpintería",
                Group = DivisionMantenimiento
            },
            new SelectListItem
            {
                Value = "HER",
                Text = "Herrería",
                Group = DivisionMantenimiento
            },
            new SelectListItem
            {
                Value = "CER",
                Text = "Cerrajería",
                Group = DivisionMantenimiento
            },
            new SelectListItem
            {
                Value = "VID",
                Text = "Vidrios",
                Group = DivisionMantenimiento
            },
            new SelectListItem
            {
                Value = "PIN",
                Text = "Pintura",
                Group = DivisionMantenimiento
            },
            new SelectListItem
            {
                Value = "DUR",
                Text = "Durlock",
                Group = DivisionMantenimiento
            },
        };
        }
        public string Division { get; set; }
        public List<SelectListItem> Divisiones { get; }
    }
}