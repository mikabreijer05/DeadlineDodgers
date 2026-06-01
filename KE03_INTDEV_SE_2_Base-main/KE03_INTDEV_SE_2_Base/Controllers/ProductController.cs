using KE03_INTDEV_SE_2_Base_main.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace KE03_INTDEV_SE_2_Base_main.Controllers
{
    public class ProductController : Controller
    {
        private static List<Product> _products = new List<Product>
        {
            new Product { Id = 1, Name = "Red Pill", Category = "Reality", Price = 999.99M, ImageUrl = "/img/redpill.png", Description = "Wake up from the simulation." },
            new Product { Id = 2, Name = "Blue Pill", Category = "Illusion", Price = 0.01M, ImageUrl = "/img/bluepill.png", Description = "Stay in wonderland." },
            new Product { Id = 3, Name = "Phone Booth", Category = "Hardware", Price = 1337.00M, ImageUrl = "/img/phone.png", Description = "Emergency exit portal." }
        };

        public IActionResult Index()
        {
            return View(_products);
        }

        // GET: /Product/Create
        public IActionResult Create()
        {
            // Leeg product zodat de view veilig aan @Model kan binden
            var newProduct = new Product();
            return View(newProduct);
        }

        // POST: /Product/Create
        [HttpPost]
        public IActionResult Create(Product newProduct)
        {
            // Basis (minimale) validatie om verbroken data te voorkomen
            if (string.IsNullOrWhiteSpace(newProduct.Name))
            {
                // Stuur dezelfde view terug met de ingevoerde waarden
                ModelState.AddModelError("Name", "Name is required.");
                return View(newProduct);
            }

            if (newProduct.Price < 0)
            {
                ModelState.AddModelError("Price", "Price cannot be negative.");
                return View(newProduct);
            }

            // Geef het product een nieuw ID (simuleert een database auto-increment)
            newProduct.Id = (_products.Count == 0) ? 1 : _products.Max(p => p.Id) + 1;

            // Voeg toe aan de in-memory lijst (dit wordt later een database insert)
            _products.Add(newProduct);

            // Ga terug naar de lijstpagina
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(Product updatedProduct)
        {
            var product = _products.FirstOrDefault(p => p.Id == updatedProduct.Id);
            if (product == null)
                return NotFound();

            product.Name = updatedProduct.Name;
            product.Category = updatedProduct.Category;
            product.Price = updatedProduct.Price;
            product.ImageUrl = updatedProduct.ImageUrl;
            product.Description = updatedProduct.Description;

            return RedirectToAction("Index");
        }
    }
}