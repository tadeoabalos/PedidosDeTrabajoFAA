using System.ComponentModel.DataAnnotations;

namespace PPTT.Models
{
    public enum DeptoEnum
    {
        [Display(Name = "División Mantenimiento de Instalaciones")]
        Generales,
        [Display(Name = "División Comunicaciones e Informática")]
        Especiales
    }
    public enum ServicioEnum
    {
        Tablero,
        [Display(Name = "Plomería")]
        Plomeria,
        [Display(Name = "Albañilería")]
        Albañileria,
        [Display(Name = "Tabiquería")]
        Tabiqueria,
        [Display(Name = "Carpintería")]
        Carpinteria,
        [Display(Name = "Herrería")]
        Herreria,
        [Display(Name = "Cerrajería")]
        Cerrajeria,
        Vidrios,
        Pintura,
        Durlock,
    }
    public enum ServiciosEnum
    {
        [Display(Name = "Telefonía")]
        Telefonia = 50
    }
}