using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using mvc.Models.Services.Application;
using mvc.Models.ViewModels;

namespace mvc.Controllers
{


    public class CoursesController : Controller
    {

        //private readonly ILogger<CoursesController> _logger;
        private readonly ICourseService cs;


        // costruttore (ctor + tab) per la dependency injection)
        public CoursesController(ICourseService courseService)
        {
            this.cs = courseService;
        }


        // public CoursesController(ILogger<CoursesController> logger)
        // {
        //     _logger = logger;
        // }

        public IActionResult Index()
        {
            // return Content(testo): per avere esatto controllo sulla risposta
            // return View(oggetto): restituisce contenuto HTML
            // return File(percorso): restituisce contenuto binario di un file
            // return Redirect(url): reindirizzare su altro url
            // return Json(ogg)=: restituire oggetto serializzato in json

            //return Content("Sono index");
            // Scarico elenco corsi dal model
            //CourseService cs = new CourseService();
            List<CourseViewModel> courses = cs.GetCourses();

            ViewData["ViewDataTitle"] = "Catalogo dei corsi";
            
            // Passo elenco alla view
            return View(courses);
        }

        public IActionResult Detail(int id)
        {
            //return Content($"Sono detail, ricevuto id {id}");
            // Dettaglio corso
            //return View();

            // Scarico corso dal model in base al ID
            //CourseService cs = new CourseService();
            var viewModel = cs.GetCourse(id);

            ViewData["ViewDataTitle"] = viewModel.Title;

            // Passo corso alla view (saltando questo passaggio si genera errore nella view)
            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }


}