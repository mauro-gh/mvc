using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace mvc.Models.InputModels
{
    // usato dal controller per aggiungere nuovo corso
    public class CourseCreateInputModel
    {
        [Required(ErrorMessage = "Testo obbligatorio"),
        MinLength(10, ErrorMessage = "Testo troppo corto, deve essere almeno {1} caratteri"),
        MaxLength(100),
        RegularExpression(@"^[\w\s\.]+$", ErrorMessage = "Caratteri non validi")]
        public string Title { get; set; } = string.Empty;
        
    }
}