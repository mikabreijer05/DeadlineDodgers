using KE03_INTDEV_SE_2_Base.DAL;
using KE03_INTDEV_SE_2_Base.Models;
using Microsoft.AspNetCore.Mvc;

namespace KE03_INTDEV_SE_2_Base_main.Controllers
{
    public class ProductController : Controller
    {
        private readonly SQLProducts _sqlProducts;

        public ProductController()
        {
            _sqlProducts = new SQLProducts();
        }

        public IActionResult Index()
        {
            var products = _sqlProducts.GetAllProducts();
            return View(products);
        }

        public IActionResult Create()
        {
            return View(new Product());
        }

        [HttpPost]
        public IActionResult Create(Product newProduct)
        {
            if (!ModelState.IsValid)
                return View(newProduct);

            _sqlProducts.AddProduct(newProduct);
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var product = _sqlProducts.GetProductById(id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(Product updatedProduct)
        {
            if (!ModelState.IsValid)
                return View(updatedProduct);

            _sqlProducts.UpdateProduct(updatedProduct);

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            _sqlProducts.DeleteProduct(id);
            return RedirectToAction("Index");
        }
    }
}