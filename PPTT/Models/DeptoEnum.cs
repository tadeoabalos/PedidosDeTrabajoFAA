using System.ComponentModel.DataAnnotations;

namespace PPTT.Models
{
    public enum DeptoEnum
    {
        [Display(Name = "División Mantenimiento de Instalaciones")]
        Generales = 2,
        [Display(Name = "División Comunicaciones e Informática")]
        Especiales = 3
    }
    public enum SerManEnum
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
    public enum SerInfEnum
    {
        [Display(Name = "Telefonía")]
        Telefonia = 50
    }
}