using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
            return Content("Sono index");
        }

        public IActionResult Detail(string id)
        {
            return Content($"Sono detail, ricevuto id {id}");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}