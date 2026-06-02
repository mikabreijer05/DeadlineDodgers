
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using KE03_INTDEV_SE_1_Base.DAL;
using KE03_INTDEV_SE_1_Base.Models;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class ProductenModel : PageModel
    {
        private readonly SQLProducts _productService;

        public IList<Product> Products { get; set; } = new List<Product>();
        public IList<string> Types { get; set; } = new List<string>();

        [BindProperty(SupportsGet = true)]
        public string? Q { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Type { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MinPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MaxPrice { get; set; }

        public ProductenModel(SQLProducts productService)
        {
            _productService = productService;
        }

        public void OnGet()
        {
            var allProducts = _productService.GetAllProducts().ToList();

            Types = allProducts
                .Where(p => !string.IsNullOrWhiteSpace(p.Category))
                .Select(p => p.Category)
                .Distinct()
                .OrderBy(category => category)
                .ToList();

            // NOTE: adjust the category value used to identify "products" to match your data.
            var filteredProducts = allProducts.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(Q))
            {
                filteredProducts = filteredProducts.Where(p =>
                    p.Name.Contains(Q, StringComparison.OrdinalIgnoreCase) ||
                    (p.Description != null && p.Description.Contains(Q, StringComparison.OrdinalIgnoreCase)));
            }

            if (!string.IsNullOrWhiteSpace(Type))
            {
                filteredProducts = filteredProducts.Where(p =>
                    p.Category != null && p.Category.Equals(Type, StringComparison.OrdinalIgnoreCase));
            }

            if (MinPrice.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.Price >= MinPrice.Value);
            }

            if (MaxPrice.HasValue)
            {
                filteredProducts = filteredProducts.Where(p => p.Price <= MaxPrice.Value);
            }

            Products = filteredProducts.ToList();
        }

        public IActionResult OnGetSearch(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return new JsonResult(new List<object>());
            }

            var results = _productService.GetAllProducts()
                .Where(p => p.Name.Contains(term, StringComparison.OrdinalIgnoreCase))
                .Take(6)
                .Select(p => new { id = p.Id, name = p.Name, price = p.Price, imageUrl = p.ImageUrl })
                .ToList();

            return new JsonResult(results);
        }
    }
}