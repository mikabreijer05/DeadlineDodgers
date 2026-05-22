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

        public ProductenModel(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public void OnGet()
        {
            Products = _productRepository.GetAllProducts().Where(p => p.Type == "product").ToList();
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
