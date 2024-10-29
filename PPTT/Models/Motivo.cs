using Microsoft.VisualBasic;

namespace PPTT.Models
{
    public class Motivo
    {
        public int ID_Pedido_de_Trabajo_Fk { get; set; }        
        public int ID_Motivo_Pk { get; set; }
        public int ID_Estado_Fk { get; set; }
        public DateTime Fecha { get; set; }
        public string? motivo { get; set; }        
    }
}
