using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class OnderdelenModel : PageModel
    {
        private readonly IProductRepository _productRepository;

        public IList<Product> Products { get; set; } = new List<Product>();

        [BindProperty(SupportsGet = true)]
        public string? Q { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MinPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MaxPrice { get; set; }

        public OnderdelenModel(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public void OnGet()
        {
            var filteredParts = _productRepository.GetAllProducts()
                .Where(p => p.Type == "onderdeel")
                .AsEnumerable();

            if (!string.IsNullOrWhiteSpace(Q))
            {
                filteredParts = filteredParts.Where(p =>
                    p.Name.Contains(Q, System.StringComparison.OrdinalIgnoreCase) ||
                    p.Description.Contains(Q, System.StringComparison.OrdinalIgnoreCase));
            }

            if (MinPrice.HasValue)
            {
                filteredParts = filteredParts.Where(p => p.Price >= MinPrice.Value);
            }

            if (MaxPrice.HasValue)
            {
                filteredParts = filteredParts.Where(p => p.Price <= MaxPrice.Value);
            }

            Products = filteredParts.ToList();
        }
    }
}