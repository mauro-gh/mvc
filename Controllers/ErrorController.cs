using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using mvc.Models.Exceptions;
using mvc.Models.ViewModels;

namespace mvc.Controllers
{

    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {

            _logger.LogWarning("gestione dell'errore");

            // in base all'eccezione e' possibile personalizzare la pagina di errore
            ErrorViewModel  errorViewModel = new ErrorViewModel();
            
            // si legge il tipo di eccezione (anche custom)
            var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            switch (feature.Error)
            {
                case CourseNotFoundException exc:
                    ViewData["Title"] = "Corso inesistente";
                    Response.StatusCode = 404;
                    return View("CourseNotFound", errorViewModel);

                case SendMailException exc:
                    ViewData["Title"] = "Errore invio messaggio";
                    Response.StatusCode=500;
                    return View();


                default:
                    ViewData["Title"] = "Errore";
                    return View();

            }
            
            
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}