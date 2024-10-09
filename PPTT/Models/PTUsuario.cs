using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPTT.Models
{
    [Table("orden_de_trabajo_usuario")]
    public class PTUsuario
    {
        [Display(Name = "ID Orden de Trabajo")]
        public int ID_Orden_Trabajo_Pk { get; set; } = 0;

        [Display(Name = "Fecha de alta")]
        public DateTime? Fecha_Subida { get; set; } = DateTime.Now;

        [Display(Name = "IP")]
        public string? IP_Solicitante { get; set; } = string.Empty;

        [Display(Name = "Nombre")]
        public string? Nombre_Solicitante { get; set; } = string.Empty;

        [Display(Name = "Apellido")]
        public string? Apellido_Solicitante { get; set; } = string.Empty;

        [Display(Name = "Grado")]
        public int ID_Grado_Fk { get; set; } = 0;

        [Display(Name = "Correo Institucional")]
        public string? Correo { get; set; } = string.Empty;

        [Display(Name = "Organismo")]
        public int ID_Organismo_Fk { get; set; }
        [ForeignKey("ID_Organismo_Fk")]
        public Organismo? Organismo { get; set; }

        [Display(Name = "RTI")]
        public string? RTI_Solicitante { get; set; } = string.Empty;

        [Display(Name = "Color del sector")]
        public string? Color_Oficina_PT { get; set; } = string.Empty;

        [Display(Name = "Número de Oficina")]
        public string? Numero_Oficina_PT { get; set; } = string.Empty;

        [Display(Name = "Piso de la Oficina")]
        public string? Piso_Oficina_PT { get; set; } = string.Empty;

        [Display(Name = "Color de la Oficina del Solicitante")]
        public string? Color_Oficina_Solicitante { get; set; } = string.Empty;

        [Display(Name = "Número de Oficina del Solicitante")]
        public string? Numero_Oficina_Solicitante { get; set; } = string.Empty;

        [Display(Name = "Piso de la Oficina del Solicitante")]
        public string? Piso_Oficina_Solicitante { get; set; } = string.Empty;

        [Display(Name = "Dependencia Interna")]
        public int ID_Dependencia_Interna_Fk { get; set; }
        [ForeignKey("ID_Dependencia_Interna_Fk")]
        public Dependencia_Interna? Dependencia_Interna { get; set; } 
        [Display(Name = "DNI del Solicitante")]
        public int DNI_Solicitante { get; set; } = 0;

        [Display(Name = "Número de Control del Solicitante")]
        public string? Numero_Control_Solicitante { get; set; } = string.Empty;

        [Display(Name = "Fecha Estimada de Finalización")]
        public DateTime? Fecha_Estimada_Fin { get; set; } = DateTime.Now;

        [Display(Name = "Fecha de Inicio Suspendido")]
        public DateTime? Fecha_Inicio_Suspendido { get; set; } = DateTime.Now;

        [Display(Name = "Prioridad")]
        public int Prioridad { get; set; } = 1;

        [Display(Name = "Estado")]
        public int ID_Estado_Fk { get; set; } = 1002;
        [ForeignKey("ID_Estado_Fk")]
        public Estado? Estado { get; set; }
        [Display(Name = "Tareas")]
        public int ID_Tarea_Fk { get; set; }
        [ForeignKey("ID_Tarea_Fk")]
        public Tarea? Tarea { get; set; }
        [Display(Name = "Observaciones")] 
        public string? Observacion { get; set; }
    }
}
