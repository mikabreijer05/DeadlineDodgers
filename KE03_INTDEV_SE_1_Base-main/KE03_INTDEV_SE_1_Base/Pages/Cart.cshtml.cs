
using KE03_INTDEV_SE_1_Base.DAL;
using KE03_INTDEV_SE_1_Base.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class cartModel : PageModel
    {
        private readonly SQLOrder _orderService;
        private readonly SQLProducts _productService;

        public cartModel(SQLOrder orderService, SQLProducts productService)
        {
            _orderService = orderService;
            _productService = productService;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPostCheckout([FromBody] CheckoutRequest request)
        {
            if (!Request.Cookies.TryGetValue("LoggedInCustomerId", out var customerIdCookie) ||
                !int.TryParse(customerIdCookie, out var customerId))
            {
                return new JsonResult(new { success = false, message = "Je moet ingelogd zijn om te bestellen." })
                {
                    StatusCode = 401
                };
            }

            if (request.Items == null || request.Items.Count == 0)
            {
                return new JsonResult(new { success = false, message = "Je winkelwagen is leeg." })
                {
                    StatusCode = 400
                };
            }

            var order = new Order
            {
                CustomerId = customerId,
                OrderDate = DateTime.Now
            };

            foreach (var cartItem in request.Items)
            {
                var product = _productService.GetProductById(cartItem.ProductId);
                if (product == null)
                {
                    continue;
                }

                order.OrderLines.Add(new OrderLine
                {
                    ProductId = product.Id,
                    Quantity = cartItem.Quantity,
                    PricePerProduct = product.Price
                });
            }

            if (!order.OrderLines.Any())
            {
                return new JsonResult(new { success = false, message = "Geen geldige producten gevonden." })
                {
                    StatusCode = 400
                };
            }

            _orderService.AddOrder(order);

            return new JsonResult(new { success = true });
        }
    }

    public class CheckoutRequest
    {
        public List<CheckoutItemRequest> Items { get; set; } = new List<CheckoutItemRequest>();
    }

    public class CheckoutItemRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
