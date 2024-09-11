using System.Globalization;
using System.ComponentModel.DataAnnotations;
namespace PPTT.Models
{
    public class Admin
    {
        [Display(Name = "Nombre")]
        public int Id { get; set; }
        [Display(Name = "Nombre")]
        public string? Name { get; set; }
        [Display(Name = "Apellido")]
        public string? Sname { get; set; }
        [Display(Name = "Numero de documento")]
        public int Dni { get; set; }
        [Display(Name = "Numero de control")]
        public int NumeroControl { get; set; }
        [Display(Name = "Correo institucional")]
        public string? Email { get; set; }
        [Display(Name = "División")]
        public int? Division {  get; set; }
        [Display(Name = "Servicio")]
        public string? Servicio { get; set; }        
    }
}
