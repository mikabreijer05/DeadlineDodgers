
using KE03_INTDEV_SE_1_Base.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class OrderHistoryModel : PageModel
    {
        private readonly SQLOrder _orderService;

        public OrderHistoryModel(SQLOrder orderService)
        {
            _orderService = orderService;
        }

        public List<OrderHistoryViewModel> Orders { get; set; } = new List<OrderHistoryViewModel>();

        public IActionResult OnGet()
        {
            if (!Request.Cookies.TryGetValue("LoggedInCustomerId", out var customerIdCookie) ||
                !int.TryParse(customerIdCookie, out var customerId))
            {
                return RedirectToPage("/Index");
            }

            Orders = _orderService.GetOrdersByCustomerId(customerId)
                .Select(order => new OrderHistoryViewModel
                {
                    OrderNumber = order.Id.ToString(),
                    OrderDate = order.OrderDate,
                    Products = order.OrderLines.Select(orderLine => new OrderProductViewModel
                    {
                        ProductName = orderLine.Product.Name, // Ensure this matches the product name in OrderLine
                        Quantity = orderLine.Quantity,       // Ensure this matches the quantity in OrderLine
                        PricePerProduct = orderLine.Product.Price, // Ensure this matches the price per product in OrderLine
                    }).ToList()
                })
                .ToList();
            return Page();
        }
    }

    public class OrderHistoryViewModel
    {
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderProductViewModel> Products { get; set; } = new List<OrderProductViewModel>();
        public decimal TotalPrice => Products.Sum(product => product.TotalPrice);
    }

    public class OrderProductViewModel
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerProduct { get; set; }

        // Calculate total price for each product
        public decimal TotalPrice => Quantity * PricePerProduct;
    }
}