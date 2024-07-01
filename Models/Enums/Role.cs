using System.ComponentModel.DataAnnotations;

namespace mvc.Models.Enums
{
    public enum Role
    {
        [Display(Name = "Amministratore")]
        Administrator,
        [Display(Name = "Docente")]
        Teacher
    }
}