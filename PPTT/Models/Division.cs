using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPTT.Models
{
    [Table("division_mantenimiento")] // Esto especifica el nombre de la tabla en la base de datos.
    public class Division
    {
        [Key] // Asegúrate de que la propiedad clave esté marcada correctamente.
        public int? ID_Division_Pk { get; set; }
        public string? Descripcion_Division { get; set; }
    }
}
