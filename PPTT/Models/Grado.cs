using System.ComponentModel.DataAnnotations;

namespace PPTT.Models
{
    public enum Grados
    {
        [Display(Name = "Brigadier General")]
        BrigadierGeneral,
        [Display(Name = "Brigadier Mayor")]
        BrigadierMayor,
        [Display(Name = "Brigadier")]
        Brigadier,
        [Display(Name = "Comodoro Mayor")]
        ComodoroMayor,
        [Display(Name = "Comodoro")]
        Comodoro,
        [Display(Name = "Vice Comodoro")]
        ViceComodoro,
        [Display(Name = "Mayor")]
        Mayor,
        [Display(Name = "Capitán")]
        Capitán,
        [Display(Name = "Primer Teniente")]
        PrimerTeniente,
        [Display(Name = "Teniente")]
        Teniente,
        [Display(Name = "Alférez")]
        Alférez,
        [Display(Name = "Suboficial Mayor")]
        SuboficialMayor,
        [Display(Name = "Suboficial Principal")]
        SuboficialPrincipal,
        [Display(Name = "Suboficial Ayudante")]
        SuboficialAyudante,
        [Display(Name = "Suboficial Auxiliar")]
        SuboficialAuxiliar,
        [Display(Name = "Cabo Principal")]
        CaboPrincipal,
        [Display(Name = "Cabo Primero")]
        CaboPrimero,
        [Display(Name = "Cabo")]
        Cabo,
        [Display(Name = "Voluntario de Primera")]
        VoluntarioDePrimera,
        [Display(Name = "Voluntario de Segunda")]
        VoluntarioDeSegunda,
        [Display(Name = "Personal Civil")]
        PersonalCivil
    }
}
