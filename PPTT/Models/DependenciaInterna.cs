namespace PPTT.Models
{
    public class DependenciaInterna
    {
        public int ID_Dependencia_Interna_PK { get; set; }
        public int ID_Organismo_FK { get; set; }
        public string? Descripcion_Dependencia { get; set; }
    }
}
