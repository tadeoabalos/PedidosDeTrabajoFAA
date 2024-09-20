using System.Globalization;
using System.ComponentModel.DataAnnotations;
namespace PPTT.Models
{
    public class Admin
    {        
        public int Id { get; set; }
        [Display(Name = "Nombre")]
        public string? Name { get; set; }
        [Display(Name = "Apellido")]
        public string? Sname { get; set; }
        [Display(Name = "Correo institucional")]
        public string? Email { get; set; }
        [Display(Name = "Rol")]
        public int? Rol { get; set; }        
        [Display(Name = "Contraseña")]
        public string? Password { get; set; }
        [Display(Name = "Numero de documento")]
        public int Dni { get; set; }
        [Display(Name = "Numero de control")]
        public string? NumeroControl { get; set; }
        [Display(Name = "División")]
        public int? DivisionFk {  get; set; }
        [Display(Name = "Servicio")]
        public int? ServicioFk { get; set; }        
    }
}
