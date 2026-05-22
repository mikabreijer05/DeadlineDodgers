using Microsoft.AspNetCore.Mvc.RazorPages;
using DataAccessLayer.Models;
using DataAccessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class ProductPaginaModel : PageModel
    {
        private readonly IProductRepository _productRepository;
        private readonly IReviewRepository _reviewRepository;

        public ProductPaginaModel(IProductRepository productRepository, IReviewRepository reviewRepository)
        {
            _productRepository = productRepository;
            _reviewRepository = reviewRepository;
        }

        public Product Product { get; set; } = new Product();
        public IEnumerable<Review> Reviews { get; set; } = new List<Review>();
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }

        public IActionResult OnGet(int id)
        {
            var product = _productRepository.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }

            Product = product;
            Reviews = _reviewRepository.GetReviewsByProductId(id);
            AverageRating = _reviewRepository.GetAverageRating(id);
            ReviewCount = _reviewRepository.GetReviewCount(id);
            return Page();
        }
    }
}
