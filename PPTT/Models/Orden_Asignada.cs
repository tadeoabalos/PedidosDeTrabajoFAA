namespace PPTT.Models
{
    public class Orden_Asignada
    {
        public int ID_Trabajo_Asignado_Pk { get; set; } 
        public int? ID_Orden_Trabajo_Fk { get; set; }
        public int? ID_Usuario_Fk { get; set; }
    }
}
