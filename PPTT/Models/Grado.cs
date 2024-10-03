using System.ComponentModel.DataAnnotations.Schema;

namespace PPTT.Models
{
    [Table("grado")]
    public class Grado
    {
        public int ID_Grado_PK { get; set; }
        public string? Descripcion_Grado { get; set; }
    }
}
