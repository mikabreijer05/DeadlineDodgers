using KE03_INTDEV_SE_2_Base.Models;
using KE03_INTDEV_SE_2_Base_main.Models; // Make sure this matches your project's namespace!
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace KE03_INTDEV_SE_2_Base_main.Controllers
{
    public class HomeController : Controller   // <---- Correct inheritance here!
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}