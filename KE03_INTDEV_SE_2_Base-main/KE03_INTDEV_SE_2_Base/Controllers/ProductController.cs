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
            // I create an instance of my DAL here so I can access database methods
            // This is simple but not ideal long-term (normally I would use dependency injection)
            _sqlProducts = new SQLProducts();
        }

        public IActionResult Index()
        {
            // I retrieve all products from my database through the DAL
            // I pass them directly into the view so Razor can render them
            var products = _sqlProducts.GetAllProducts();

            return View(products);
        }

        public IActionResult Create()
        {
            // I return an empty Product object so my view has a model to bind to
            // This prevents null reference issues in the Razor view
            return View(new Product());
        }

        [HttpPost]
        public IActionResult Create(Product newProduct)
        {
            // I validate the incoming data before sending it to the database
            // This prevents invalid records from being stored

            if (string.IsNullOrWhiteSpace(newProduct.Name))
            {
                ModelState.AddModelError("Name", "Name is required.");
                return View(newProduct);
            }

            if (newProduct.Price < 0)
            {
                ModelState.AddModelError("Price", "Price cannot be negative.");
                return View(newProduct);
            }

            if (!ModelState.IsValid)
            {
                return View(newProduct);
            }

            // I insert the new product into the database using my DAL
            _sqlProducts.AddProduct(newProduct);

            // After successful insert, I redirect back to the product overview
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            // I fetch a single product by id so I can edit it
            var product = _sqlProducts.GetProductById(id);

            // I check if the product exists to avoid errors when an invalid id is passed
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(Product updatedProduct)
        {
            // I validate the model again before updating the database
            if (!ModelState.IsValid)
            {
                return View(updatedProduct);
            }

            // I update the product in the database using my DAL
            _sqlProducts.UpdateProduct(updatedProduct);

            // After updating, I go back to the product list
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            // I remove the product from the database using its id
            _sqlProducts.DeleteProduct(id);

            // After deletion, I return to the overview page
            return RedirectToAction("Index");
        }
    }
}