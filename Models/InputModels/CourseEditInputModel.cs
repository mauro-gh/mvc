using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mvc.Controllers;
using mvc.Models.Entities;
using mvc.Models.Enums;
using mvc.Models.ValueObjects;

namespace mvc.Models.InputModels
{
    // l'interfaccia serve per validazioni che interessano piu' campi
    public class CourseEditInputModel : IValidatableObject
    {


        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Il titolo è obbligatorio"),
        MinLength(10, ErrorMessage = "Il titolo dev'essere di almeno {1} caratteri"),
        MaxLength(100, ErrorMessage = "Il titolo dev'essere di al massimo {1} caratteri"),
        RegularExpression(@"^[0-9A-z\u00C0-\u00ff\s\.']+$", ErrorMessage = "Titolo non valido"), //Questa espressione regolare include anche i caratteri accentati
        Remote(action: nameof(CoursesController.IsTitleAvailable), controller: "Courses", ErrorMessage = "Il titolo esiste già (ajax)", AdditionalFields = "Id"),
        Display(Name = "Titolo")]
        public string Title { get; set; }
        
        [MinLength(10, ErrorMessage = "La descrizione dev'essere di almeno {1} caratteri"),
        MaxLength(4000, ErrorMessage = "La descrizione dev'essere di massimo {1} caratteri"),
        Display(Name = "Descrizione")]
        public string Description { get; set; }

        [Display(Name = "Immagine rappresentativa")]
        public string LogoPath { get; set; }

        [Required(ErrorMessage = "L'email di contatto è obbligatoria"),
        EmailAddress(ErrorMessage = "Devi inserire un indirizzo email"),
        Display(Name = "Email di contatto")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Il prezzo intero è obbligatorio"),
        Display(Name = "Prezzo intero")]
        public Money FullPrice {get;set; }

        [Required(ErrorMessage = "Il prezzo corrente è obbligatorio"),
        Display(Name = "Prezzo corrente")]
        public Money CurrentPrice {get;set; }

        [Display(Name = "Nuova immagine...")]
        public IFormFile? Image {get;set;} = null;
        
        public string? RowVersion { get; set; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // validazione valuta
            if (FullPrice.Currency != CurrentPrice.Currency)
            {
                yield return new ValidationResult("Il prezzo intero deve avere la stessa valuta del prezzo corrente", new[] { nameof(FullPrice) });
            }
            // validazione prezzo
            else if (FullPrice.Amount < CurrentPrice.Amount)
            {
                yield return new ValidationResult("Il prezzo intero non può essere inferiore al prezzo corrente", new[] { nameof(FullPrice) });
            }            
        }

        public static CourseEditInputModel FromDataRow(DataRow courseRow)
        {
            var courseEditInputModel = new CourseEditInputModel
            {
                Title = Convert.ToString(courseRow["Title"]),
                Description = Convert.ToString(courseRow["Description"]),
                LogoPath = Convert.ToString(courseRow["LogoPath"]),
                Email = Convert.ToString(courseRow["Email"]),
                FullPrice = new Money(
                    Enum.Parse<Currency>(Convert.ToString(courseRow["FullPrice_Currency"])),
                    Convert.ToDecimal(courseRow["FullPrice_Amount"])
                ),
                CurrentPrice = new Money(
                    Enum.Parse<Currency>(Convert.ToString(courseRow["CurrentPrice_Currency"])),
                    Convert.ToDecimal(courseRow["CurrentPrice_Amount"])
                ),
                Id = Convert.ToInt32(courseRow["Id"]),
                RowVersion = Convert.ToString(courseRow["RowVersion"])
            };
            // if (string.IsNullOrEmpty(courseEditInputModel.LogoPath))
            // {
            //     courseEditInputModel.LogoPath = @"/Courses/default.png";
            // }
            return courseEditInputModel;
        }

        public static CourseEditInputModel FromEntity(Course course)
        {
            return new CourseEditInputModel
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Email = course.Email,
                LogoPath = course.LogoPath,
                CurrentPrice = course.CurrentPrice,
                FullPrice = course.FullPrice,
                RowVersion = course.RowVersion
            };
        }
    
    }
}