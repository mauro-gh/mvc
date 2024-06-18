using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mvc.Models;
using mvc.Models.Services.Application;
using mvc.Models.ViewModels;

namespace mvc.Controllers;

// Attributo per mettere nella cache del browser questo intero controller

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
        
    }


    // Attributo per mettere nella cache del browser questo action
    // FromService: serve a dire al model binding che questa istanza deve essere cercata
    // tra i servizi registrati per la dependeny injection
    [AllowAnonymous, ResponseCache(CacheProfileName = "Home")]
    public async Task<IActionResult> Index([FromServices] ICachedCourseService courseService)
    {
        ViewData["ViewDataTitle"] = "Benvenuti";

        // dobbiamo ottenere lista nuovi corsi e lista corsi migliori,
        // ma sempre dal servizio applicativo e non da questo controller

        List<CourseViewModel> listaMiglioriCorsi = await courseService.GetBestRatingCoursesAsync();
        List<CourseViewModel> listaCorsiRecenti = await courseService.GetMostRecentCoursesAsync();

        // Creo un view model che raccolta entrambe le liste, da mostrare dal html
        HomeViewModel viewModel = new HomeViewModel
        {
            BestRatingCourses = listaMiglioriCorsi,
            MostRecentCourses = listaCorsiRecenti
        };

        return View(viewModel);
        //return Content("Sono index della home controller");
    }

    // public IActionResult Privacy()
    // {
    //     return View();
    // }

    // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    // public IActionResult Error()
    // {
    //     return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    // }
}
