using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using mvc.Models;

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
    [ResponseCache(CacheProfileName = "Home")]
    public IActionResult Index()
    {
        ViewData["ViewDataTitle"] = "Benvenuti";

        return View();
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
