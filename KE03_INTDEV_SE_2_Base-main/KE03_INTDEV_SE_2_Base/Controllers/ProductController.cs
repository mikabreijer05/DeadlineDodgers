using KE03_INTDEV_SE_2_Base_main.Models; // Stick with your own model for learning!
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace KE03_INTDEV_SE_2_Base_main.Controllers
{
    public class ProductController : Controller
    {
        private static List<Product> _products = new List<Product>
        {
            new Product{ Id = 1, Name="Monitor", Price=199.99M },
            new Product{ Id = 2, Name="Keyboard", Price=39.95M },
            new Product{ Id = 3, Name="Mouse", Price=25.00M }
        };

        // /Product/List
        public IActionResult List()
        {
            return View(_products);
        }

        // /Product/Detail/2
        public IActionResult Detail(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();
            return View(product);
        }
    }
}