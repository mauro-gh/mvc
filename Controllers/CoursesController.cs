using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using mvc.Models.Exceptions;
using mvc.Models.InputModels;
using mvc.Models.Services.Application;
using mvc.Models.ViewModels;

namespace mvc.Controllers
{


    public class CoursesController : Controller
    {

        //private readonly ILogger<CoursesController> _logger;
        private readonly ICourseService cs;


        // costruttore (ctor + tab) per la dependency injection)
        public CoursesController(ICachedCourseService courseService)
        {
            this.cs = courseService;
        }


        // public CoursesController(ILogger<CoursesController> logger)
        // {
        //     _logger = logger;
        // }

        // parametri prima: string search = null, int page = 1, string orderby = "CurrentPrice",bool ascending = true
        public async Task<IActionResult> Index([FromQuery]CourseListInputModel model)
        {
            // questo metodo si occupa anche della ricerca
            // TODO, sostituire tutti i parametri col modello
            ListViewModel<CourseViewModel> courses = await cs.GetCoursesAsync(model);

            ViewData["ViewDataTitle"] = "Catalogo dei corsi";

            CourseListViewModel viewModel = new CourseListViewModel();
            viewModel.Courses = courses;
            viewModel.Input =  model;
            
            // Passo elenco alla view
            return View(viewModel);          
        }

        // public async Task<IActionResult> Index2()
        // {
        //     // return Content(testo): per avere esatto controllo sulla risposta
        //     // return View(oggetto): restituisce contenuto HTML
        //     // return File(percorso): restituisce contenuto binario di un file
        //     // return Redirect(url): reindirizzare su altro url
        //     // return Json(ogg)=: restituire oggetto serializzato in json

        //     //return Content("Sono index");
        //     // Scarico elenco corsi dal model
        //     //CourseService cs = new CourseService();
        //     List<CourseViewModel> courses = await cs.GetCoursesAsync();

        //     ViewData["ViewDataTitle"] = "Catalogo dei corsi";
            
        //     // Passo elenco alla view
        //     return View(courses);
        // }

        public async Task<IActionResult> Detail([FromRoute]int id)
        {
            //return Content($"Sono detail, ricevuto id {id}");
            // Dettaglio corso
            //return View();

            // Scarico corso dal model in base al ID
            //CourseService cs = new CourseService();
            var viewModel = await cs.GetCourseAsync(id);

            ViewData["ViewDataTitle"] = viewModel.Title;

            // Passo corso alla view (saltando questo passaggio si genera errore nella view)
            return View(viewModel);
        }

        // questa action ha l'unica responsabilita' di visualizzare una view vuota, e' in get
        // non serve sia async, non legge da DB
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["Title"] = "Inserimento nuovo corso (GET)";
            // model binding (da form a entita)
            var inputModel = new CourseCreateInputModel();
            return View(inputModel);
        }


        // questa invece e' quella che riceve i valori del form di nuovo corso, e' in post
        [HttpPost]
        public async Task<IActionResult> Create([FromForm]CourseCreateInputModel inputModel){
            // validazioni (col ModelState)

            if (ModelState.IsValid)
            {
                try
                {
                    // TUTTO OK
                    // utilizzare il servizio applicativo per salvare il corso
                    CourseDetailViewModel course = await cs.CreateCourseAsync(inputModel);
                    TempData["MessaggioDiConferma"] = "Corso creato, ora compila gli altri campi!";
                    //return RedirectToAction(nameof(Index));
                    return RedirectToAction(nameof(Edit), new {id = course.Id}); // verra' passato al routing per comporre la pag detail/19
                }
                catch (CourseTitleDuplicateException ex)
                {
                    // DUPLICATO, aggiungiamo errore
                    ModelState.AddModelError("Title", ex.Message);                     
                }
            }

            ViewData["Title"] = "Inserimento nuovo corso (POST)";
            // restituiamo all'utente il suo input non valido
            return View(inputModel);    

        }

        public async Task<IActionResult> IsTitleAvailable(string title, int id=0)
        {
            bool disponibile = await cs.IsTitleAvailableAsync(title, id);
            // serve per ajax, validazione di tipo Remote, deve restiture un json booleano
            return Json(disponibile);

        }

        // questa action ha la responsabilita' di modificare un corso, in GET vieno solo mostrata la view
        [HttpGet]
        public  async Task<IActionResult> Edit(int id)
        {
            ViewData["Title"] = $"Modifica corso {id} (GET)";
            // model binding, ci facciamo restituire dal S.A. un'istanza gia' popolata
            CourseEditInputModel inputModel = await cs.GetCourseForEditingAsync(id);
            // view fortemente tipizzata
            return View(inputModel);
        }        

        // e in POST viene gestito l'input dell'utente, grazie al model binding
        [HttpPost]
        public  async Task<IActionResult> Edit([FromForm]CourseEditInputModel inputModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // TUTTO OK
                    // utilizzare il servizio applicativo per salvare il corso
                    CourseDetailViewModel course = await cs.SaveCourseAsync(inputModel);
                    TempData["MessaggioDiConferma"] = "Dati salvati su DB";
                    return RedirectToAction(nameof(Detail), new {id = inputModel.Id}); // verra' passato al routing per comporre la pag detail/19
                }
                catch (CourseNotFoundException)
                {
                    // DUPLICATO, aggiungiamo errore
                    ModelState.AddModelError("Title", "Eccezione CourseNotFoundException");                     
                }
                catch (OptimisticConcurrencyException)
                {
                    // DUPLICATO, aggiungiamo errore
                    ModelState.AddModelError("Title", "Ecezione OptimisticConcurrencyException");                     
                }
                catch (Exception ex)
                {
                    // DUPLICATO, aggiungiamo errore
                    ModelState.AddModelError("Title", ex.Message);                     
                }
            }

            ViewData["Title"] = $"Modifica corso (POST)";
            // restituiamo all'utente il suo input non valido
            return View(inputModel);    
        }  



        [HttpPost]
        public  async Task<IActionResult> Delete([FromForm]CourseDeleteInputModel inputModel)
        {
            await cs.DeleteCourseAsync (inputModel);
            TempData["MessaggioDiConferma"] = "Corso cancellato, attendere un minuto";
            //redirect to lista corsi
            return RedirectToAction("Index"); //meglio il nameof
        }

        #region errors

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }



        #endregion
    }


}