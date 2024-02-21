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
        private readonly ILogger<CoursesController> _logger;

        public CoursesController(ILogger<CoursesController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // return Content(testo): per avere esatto controllo sulla risposta
            // return View(oggetto): restituisce contenuto HTML
            // return File(percorso): restituisce contenuto binario di un file
            // return Redirect(url): reindirizzare su altro url
            // return Json(ogg)=: restituire oggetto serializzato in json

            //return Content("Sono index");
            // Elenco corsi
            CourseService cs = new CourseService();
            List<CourseViewModel> courses = cs.GetServices();
            
            // Passo elenco alla view
            return View(courses);
        }

        public IActionResult Detail(string id)
        {
            //return Content($"Sono detail, ricevuto id {id}");
            // Dettaglio corso
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}