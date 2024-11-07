using System.ComponentModel.DataAnnotations.Schema;

namespace PPTT.Models
{
    [Table("orden_asignada_usuario")]
    public class Orden_Asignada_Usuario
    {
        public int ID_Trabajo_Asignado_Pk { get; set; }
        public int? ID_Orden_Trabajo_Fk { get; set; }
        public int? ID_Usuario_Fk { get; set; }
    }
}
