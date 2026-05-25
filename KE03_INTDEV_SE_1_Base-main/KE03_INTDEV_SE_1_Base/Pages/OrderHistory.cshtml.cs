using DataAccessLayer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class OrderHistoryModel : PageModel
    {
        private readonly IOrderRepository _orderRepository;

        public OrderHistoryModel(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public List<OrderHistoryViewModel> Orders { get; set; } = new List<OrderHistoryViewModel>();

        public IActionResult OnGet()
        {
            if (!Request.Cookies.TryGetValue("LoggedInCustomerId", out var customerIdCookie) ||
                !int.TryParse(customerIdCookie, out var customerId))
            {
                return RedirectToPage("/Index");
            }

            Orders = _orderRepository.GetOrdersByCustomerId(customerId)
                .Select(order => new OrderHistoryViewModel
                {
                    OrderNumber = order.Id.ToString(),
                    OrderDate = order.OrderDate,
                    Products = order.OrderLines.Select(orderLine => new OrderProductViewModel
                    {
                        ProductName = orderLine.Product.Name,
                        Quantity = orderLine.Quantity,
                        PricePerProduct = orderLine.PricePerProduct
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

        public decimal TotalPrice => Quantity * PricePerProduct;
    }
}