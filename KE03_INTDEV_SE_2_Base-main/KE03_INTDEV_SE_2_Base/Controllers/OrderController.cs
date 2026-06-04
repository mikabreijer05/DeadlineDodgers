using Microsoft.AspNetCore.Mvc;

namespace KE03_INTDEV_SE_2_Base.Models;

public class OrderController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}