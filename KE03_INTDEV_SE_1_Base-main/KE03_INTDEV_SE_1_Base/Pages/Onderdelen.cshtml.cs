
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using KE03_INTDEV_SE_1_Base.DAL;
using KE03_INTDEV_SE_1_Base.Models;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class OnderdelenModel : PageModel
    {
        private readonly SQLProducts _productService;

        public IList<Product> Products { get; set; } = new List<Product>();

        [BindProperty(SupportsGet = true)]
        public string? Q { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MinPrice { get; set; }

        [BindProperty(SupportsGet = true)]
        public decimal? MaxPrice { get; set; }

        public OnderdelenModel(SQLProducts productService)
        {
            _productService = productService;
        }

        public void OnGet()
        {
            // NOTE: adjust the category value used to identify "onderdelen" to match your data.
            var filteredParts = _productService.GetAllProducts()
                .Where(p => string.Equals(p.Category, "onderdeel", StringComparison.OrdinalIgnoreCase))
                .AsEnumerable();

            if (!string.IsNullOrWhiteSpace(Q))
            {
                filteredParts = filteredParts.Where(p =>
                    p.Name.Contains(Q, StringComparison.OrdinalIgnoreCase) ||
                    (p.Description != null && p.Description.Contains(Q, StringComparison.OrdinalIgnoreCase)));
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