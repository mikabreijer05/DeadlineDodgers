using KE03_INTDEV_SE_2_Base.DAL;
using KE03_INTDEV_SE_2_Base.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace KE03_INTDEV_SE_2_Base_main.Controllers
{
    public class ProductController : Controller
    {
        private readonly SQLProducts _sqlProducts;

        public ProductController()
        {
            _sqlProducts = new SQLProducts();
        }

        // LIST PAGE
        public IActionResult Index()
        {
            var products = _sqlProducts.GetAllProducts().ToList();
            return View(products);
        }

        // EDIT PAGE (GET)
        public IActionResult Edit(int id)
        {
            var product = _sqlProducts.GetProductById(id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        // EDIT PAGE (POST)
        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if (!ModelState.IsValid)
                return View(product);

            _sqlProducts.UpdateProduct(product);
            return RedirectToAction("Index");
        }

        // CREATE PAGE (GET)
        public IActionResult Create()
        {
            return View(new Product());
        }

        // CREATE PAGE (POST)
        [HttpPost]
        public IActionResult Create(Product newProduct)
        {
            if (!ModelState.IsValid)
                return View(newProduct);

            _sqlProducts.AddProduct(newProduct);
            return RedirectToAction("Index");
        }

        // DELETE
        public IActionResult Delete(int id)
        {
            _sqlProducts.DeleteProduct(id);
            return RedirectToAction("Index");
        }
    }
}