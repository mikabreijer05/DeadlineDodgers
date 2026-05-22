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

        public OnderdelenModel(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public void OnGet()
        {
            Products = _productRepository.GetAllProducts().Where(p => p.Type == "onderdeel").ToList();
        }
    }
}
