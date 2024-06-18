using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using mvc.Models.Services.Application;
using mvc.Models.ViewModels;

namespace mvc.Pages
{
    [Authorize]
    public class Contact : PageModel
    {

        // con le razor page si accede alle proprieta' del modello tramite @Model.Course
        public CourseDetailViewModel Course {get; private set;}

    
        // Proprieta' bindata con la page 
        [Required(ErrorMessage = "Inserire il testo nella domanda", AllowEmptyStrings = false)]
        [BindProperty]                
        public string ContactQuestion {get;set;}


        // Viene chiamato dalla Detail: https://localhost:7206/Contact?id=52
        // riceve id dalla pagina (es. 52) e ICourseService dal servizio di dependecy injection
        public async Task<IActionResult> OnGetAsync(int id, [FromServices] ICourseService cs)
        {
           
            
            try
            {
                Course = await cs.GetCourseAsync(id);    
            }
            catch (System.Exception)
            {
                
                return RedirectToAction("Index", "Courses");
            }

            return Page();
            

        }

        public async Task<IActionResult> OnPostAsync([FromQuery] int id,  [FromServices] ICourseService cs)
        {


            if (ModelState.IsValid && ContactQuestion != null)
            {
                // Invio domanda al docente
                await cs.SendQuestionToCourseAuthorAsync(id, ContactQuestion);
                


                TempData["MessaggioDiConferma"] = "Domanda correttamente inviata";
                return RedirectToAction("Detail", "Courses", new {id = id});
            }
            else
            {
                // ricarico la pagina
                return await OnGetAsync(id, cs);
            }

        }



    }
}