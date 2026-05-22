using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class ProductenModel : PageModel
    {
        private readonly IProductRepository _productRepository;

        public IList<Product> Products { get; set; } = new List<Product>();

        public IList<string> Categories { get; set; } = new List<string>();

        [BindProperty(SupportsGet = true)]
        public string? Q { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Category { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MinPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MaxPrice { get; set; }
        
        public ProductenModel(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public void OnGet()
        {
            var allProducts = _productRepository.GetAllProducts().ToList();

            Categories = allProducts
                .Where(p => !string.IsNullOrWhiteSpace(p.Type))
                .Select(p => p.Type)
                .Distinct()
                .OrderBy(type => type)
                .ToList();

            var filteredProducts = allProducts.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(Q))
            {
                filteredProducts = filteredProducts.Where(p =>
                    p.Name.Contains(Q, System.StringComparison.OrdinalIgnoreCase) ||
                    p.Description.Contains(Q, System.StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(Category))
            {
                filteredProducts = filteredProducts.Where(p =>
                    p.Type.Equals(Category, System.StringComparison.OrdinalIgnoreCase));
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

            var results = _productRepository.GetAllProducts()
                .Where(p => p.Name.Contains(term, System.StringComparison.OrdinalIgnoreCase))
                .Take(6)
                .Select(p => new
                {
                    id = p.Id,
                    name = p.Name,
                    price = p.Price,
                    imageUrl = p.ImageUrl
                })
                .ToList();

            return new JsonResult(results);
        }
    }
}
