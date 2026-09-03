
using KE03_INTDEV_SE_1_Base.DAL;
using KE03_INTDEV_SE_1_Base.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly SQLCustomer _customerService;
        private readonly SQLProducts _productService;

        public IList<Customer> Customers { get; set; }
        public IList<Product> Products { get; set; }

        public IndexModel(ILogger<IndexModel> logger, SQLCustomer customerService, SQLProducts productService)
        {
            _logger = logger;
            _customerService = customerService;
            Customers = new List<Customer>();
            _productService = productService;
            Products = new List<Product>();
        }

        public void OnGet()
        {
            Customers = _customerService.GetAllCustomers().ToList();
            Products = _productService.GetAllProducts().ToList();
            _logger.LogInformation($"getting all {Customers.Count} customers");
        }
    }
}
