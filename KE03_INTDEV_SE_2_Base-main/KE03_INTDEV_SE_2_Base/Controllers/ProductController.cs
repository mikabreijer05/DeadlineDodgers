using KE03_INTDEV_SE_2_Base.DAL;
using KE03_INTDEV_SE_2_Base.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KE03_INTDEV_SE_2_Base_main.Controllers
{
    public class ProductController : Controller
    {
        private readonly SQLProducts _sqlProducts;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _sqlProducts = new SQLProducts(configuration);
            _webHostEnvironment = webHostEnvironment;
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

            LoadProductDropdownOptions();
            return View(product);
        }
        
        private void LoadProductDropdownOptions()
        {
            ViewBag.CategoryOptions = _sqlProducts
                .GetAllCategories()
                .Select(category => new SelectListItem
                {
                    Value = category.Id.ToString(),
                    Text = category.Name
                })
                .ToList();

            ViewBag.DiscountOptions = _sqlProducts
                .GetAllDiscounts()
                .Select(discount => new SelectListItem
                {
                    Value = discount.Id.ToString(),
                    Text = discount.Name
                })
                .ToList();

            ViewBag.DimensionOptions = _sqlProducts
                .GetAllPackageDimensions()
                .Select(dimension => new SelectListItem
                {
                    Value = dimension.Id.ToString(),
                    Text = dimension.Name
                })
                .ToList();
        }
        
        [HttpPost]
        public IActionResult AddCategory(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                return BadRequest(new
                {
                    message = "Category name is required."
                });
            }

            var category = _sqlProducts.AddCategory(categoryName);

            return Json(new
            {
                id = category.Id,
                name = category.Name
            });
        }

        // EDIT PAGE (POST)
        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if (!ModelState.IsValid)
            {
                LoadProductDropdownOptions();
                return View(product);
            }

            _sqlProducts.UpdateProduct(product);
            return RedirectToAction("Index");
        }


        // CREATE PAGE (GET)
        public IActionResult Create()
        {
            LoadProductDropdownOptions();
            return View(new Product());
        }

        // CREATE PAGE (POST)
        [HttpPost]
        public IActionResult Create(Product newProduct, IFormFile? productImage)
        {
            if (productImage != null && productImage.Length > 0)
            {
                var extension = Path.GetExtension(productImage.FileName);

                if (!string.Equals(extension, ".jpg", StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("productImage", "Only .jpg files are allowed.");
                    LoadProductDropdownOptions();
                    return View(newProduct);
                }

                if (string.IsNullOrWhiteSpace(newProduct.Name))
                {
                    ModelState.AddModelError("Name", "Product name is required before uploading an image.");
                    LoadProductDropdownOptions();
                    return View(newProduct);
                }

                var uploadFolder = Path.GetFullPath(Path.Combine(
                    _webHostEnvironment.ContentRootPath,
                    "..",
                    "..",
                    "KE03_INTDEV_SE_1_Base-main",
                    "KE03_INTDEV_SE_1_Base",
                    "wwwroot",
                    "images",
                    "product"
                ));

                Directory.CreateDirectory(uploadFolder);

                var safeProductName = Regex.Replace(
                    newProduct.Name.Trim().ToLowerInvariant(),
                    @"[^a-z0-9]+",
                    "-"
                ).Trim('-');

                var safeFileName = $"{safeProductName}.jpg";
                var filePath = Path.Combine(uploadFolder, safeFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    productImage.CopyTo(fileStream);
                }

                newProduct.ImageUrl = $"images/product/{safeFileName}";
                ModelState.Remove(nameof(newProduct.ImageUrl));
            }

            if (!ModelState.IsValid)
            {
                LoadProductDropdownOptions();
                return View(newProduct);
            }

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