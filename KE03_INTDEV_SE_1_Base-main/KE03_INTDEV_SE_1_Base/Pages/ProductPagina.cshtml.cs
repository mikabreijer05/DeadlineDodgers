using System.ComponentModel.DataAnnotations;
using KE03_INTDEV_SE_1_Base.DAL;
using KE03_INTDEV_SE_1_Base.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Microsoft.AspNetCore.Mvc;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class ProductPaginaModel : PageModel
    {
        private readonly SQLProducts _productService;
        private readonly SQLReview _reviewService;

        public Product Product { get; set; } = new Product();
        public IEnumerable<Review> Reviews { get; set; } = new List<Review>();
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }

        public bool IsLoggedIn { get; set; }
        public string? LoggedInCustomerName { get; set; }

        [BindProperty]
        [Range(1, 5, ErrorMessage = "Kies een beoordeling tussen 1 en 5 sterren.")]
        public int Rating { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Vul een titel in.")]
        [StringLength(100, ErrorMessage = "De titel mag maximaal 100 tekens bevatten.")]
        public string Title { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "Vul een reviewtekst in.")]
        [StringLength(1000, ErrorMessage = "De review mag maximaal 1000 tekens bevatten.")]
        public string Comment { get; set; } = string.Empty;

        public ProductPaginaModel(SQLProducts productService, SQLReview reviewService)
        {
            _productService = productService;
            _reviewService = reviewService;
        }

        public IActionResult OnGet(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }

            Product = product;
            Reviews = _reviewService.GetReviewsByProductId(id);
            AverageRating = _reviewService.GetAverageRating(id);
            ReviewCount = _reviewService.GetReviewCount(id);

            IsLoggedIn = Request.Cookies.ContainsKey("LoggedInCustomerId");
            LoggedInCustomerName = Request.Cookies["LoggedInCustomerName"];

            return Page();
        }

        public IActionResult OnPostAddReview(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }

            var customerName = Request.Cookies["LoggedInCustomerName"];
            if (string.IsNullOrWhiteSpace(customerName))
            {
                return RedirectToPage("/Login");
            }

            Product = product;
            Reviews = _reviewService.GetReviewsByProductId(id);
            AverageRating = _reviewService.GetAverageRating(id);
            ReviewCount = _reviewService.GetReviewCount(id);
            IsLoggedIn = true;
            LoggedInCustomerName = customerName;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var review = new Review
            {
                ProductId = id,
                Username = customerName,
                Rating = Rating,
                Title = Title,
                Comment = Comment,
                CreatedDate = DateTime.Now,
                IsVerified = true
            };

            _reviewService.AddReview(review);

            return RedirectToPage("/ProductPagina", new { id });
        }
    }
}
