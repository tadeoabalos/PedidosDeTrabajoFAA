using System.Globalization;
using System.ComponentModel.DataAnnotations;

namespace PPTT.Models
{
    public class Admin
    {        
        public int ID_Usuario_Pk { get; set; }
        [Display(Name = "Nombre")]
        public string? Nombre { get; set; }
        [Display(Name = "Apellido")]
        public string? Apellido { get; set; }
        [Display(Name = "Correo institucional")]
        public string? Correo { get; set; }

        public enum Rol
        {
            // Asegúrate de tener valores definidos aquí
            Usuario = 1,
            Administrador = 2
        }

        [Display(Name = "Rol")]
        public int? ID_Rol_Fk { get; set; }     
        [Display(Name = "Contraseña")]
        public int? ID_Password_Fk { get; set; }
        [Display(Name = "Numero de documento")]
        public int DNI { get; set; }
        [Display(Name = "Numero de control")]
        public int? Numero_Control { get; set; }
        [Display(Name = "División")]
        public int? ID_Division_Fk {  get; set; }
        [Display(Name = "Servicio")]

        [Required(ErrorMessage = "Debes seleccionar un servicio.")]
        public int? ID_Servicio_Fk { get; set; }
        [Display(Name ="Fecha de Alta")]
        public DateTime? Fecha_Alta { get; set; }
        [Display(Name = "Fecha de Baja")]
        public DateTime? Fecha_Baja { get; set; }

        public string? NombreCompleto => Apellido + ", " + Nombre;
    }
}
