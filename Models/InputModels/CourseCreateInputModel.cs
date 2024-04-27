using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mvc.Controllers;

namespace mvc.Models.InputModels
{
    // usato dal controller per aggiungere nuovo corso
    public class CourseCreateInputModel
    {
        [Required(ErrorMessage = "Testo obbligatorio"),
        MinLength(10, ErrorMessage = "Testo troppo corto, deve essere almeno {1} caratteri"),
        MaxLength(100),
        RegularExpression(@"^[\w\s\.]+$", ErrorMessage = "Caratteri non validi"),
        Remote(action: nameof(CoursesController.IsTitleAvailable), controller: "Courses", ErrorMessage = "Ajax, il titolo esiste già")
        ]
        public string Title { get; set; } = string.Empty;
        
    }
}