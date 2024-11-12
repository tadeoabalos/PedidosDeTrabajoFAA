using System.Globalization;
using System.ComponentModel.DataAnnotations;

namespace PPTT.Models
{
    public class Admin
    {        
        public int ID_Usuario_Pk { get; set; }
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo Nombre es obligatorio")]
        [StringLength(50)]
        public string? Nombre { get; set; }
        [Display(Name = "Apellido")]
        [Required(ErrorMessage = "El campo Apellido es obligatorio")]
        [StringLength(50)]
        public string? Apellido { get; set; }
        [Required(ErrorMessage = "El campo Correo es obligatorio")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@faa\.mil\.ar$", ErrorMessage = "Por favor, ingresa un correo institucional válido (@faa.mil.ar).")]
        [StringLength(50)]
        [Display(Name = "Correo institucional")]
        public string? Correo { get; set; }

        public enum Rol
        {            
            Operario = 1,
            Administrador = 2,
            SuperAdministrador = 3
        }

        [Display(Name = "Rol")]
        public int? ID_Rol_Fk { get; set; }     
        [Display(Name = "Contraseña")]
        public int? ID_Password_Fk { get; set; }
        [Display(Name = "Numero de documento")]
        [Required(ErrorMessage = "El campo DNI es obligatorio")]        
        [Range(1000000, 99999999, ErrorMessage = "Número inválido de DNI")]
        public int DNI { get; set; }
        [Display(Name = "Numero de control")]
        [Required(ErrorMessage = "El campo número de control es obligatorio")]                
        public int? Numero_Control { get; set; }
        [Required(ErrorMessage = "Debe seleccionar una división.")]
        [Display(Name = "División")]
        public int? ID_Division_Fk {  get; set; }
        [Display(Name = "Servicio")]
        [Required(ErrorMessage = "Debe seleccionar un servicio.")]
        public int? ID_Servicio_Fk { get; set; }
        [Display(Name ="Fecha de Alta")]
        public DateTime? Fecha_Alta { get; set; }
        [Display(Name = "Fecha de Baja")]
        public DateTime? Fecha_Baja { get; set; }

        public string? NombreCompleto => Apellido + ", " + Nombre;
        public int? Division2 { get; set; }  // Permite que sea nulo

    }
}
