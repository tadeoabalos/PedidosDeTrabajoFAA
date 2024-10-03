namespace PPTT.Models
{
    public class Tarea
    {
        public int Id_Tarea_Pk{ get; set; }
        public string Descripcion_Tarea { get; set; }
        public int Id_Servicio_Fk { get; set; }
    }
}
