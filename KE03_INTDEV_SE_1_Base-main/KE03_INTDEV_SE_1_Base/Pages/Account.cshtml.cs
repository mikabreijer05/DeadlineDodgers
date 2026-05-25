using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class AccountModel : PageModel
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderRepository _orderRepository;

        public AccountModel(ICustomerRepository customerRepository, IOrderRepository orderRepository)
        {
            _customerRepository = customerRepository;
            _orderRepository = orderRepository;
        }

        public IActionResult OnPostLogout()
        {
            // remove cookies
            if (Request.Cookies.ContainsKey("LoggedInCustomerId"))
            {
                Response.Cookies.Delete("LoggedInCustomerId");
            }
            if (Request.Cookies.ContainsKey("LoggedInCustomerName"))
            {
                Response.Cookies.Delete("LoggedInCustomerName");
            }

            return RedirectToPage("/Index");
        }

        public UserProfile UserData { get; set; }

        public List<OrderItem> Orders { get; set; } = new List<OrderItem>();
        
        public IActionResult OnGet(int? id)
        {
            if (id == null)
            {
                if (Request.Cookies.TryGetValue("LoggedInCustomerId", out var customerIdCookie) &&
                    int.TryParse(customerIdCookie, out var loggedInCustomerId))
                {
                    id = loggedInCustomerId;
                }
                else
                {
                    return RedirectToPage("/Index");
                }
            }

            var customer = _customerRepository.GetCustomerById(id.Value);
            if (customer == null)
            {
                return NotFound();
            }

            UserData = new UserProfile
            {
                Name = customer.Name,
                Address = customer.Address,
                Active = customer.Active,
            };

            Orders = _orderRepository.GetOrdersByCustomerId(customer.Id)
                .Select(order => new OrderItem
                {
                    OrderNumber = order.Id.ToString(),
                    OrderDate = order.OrderDate,
                    ItemName = string.Join(", ", order.Products.Select(product => product.Name)),
                    Status = "Completed",
                    Price = order.Products.Sum(product => product.Price)
                })
                .ToList();

            return Page();
        }
    }

    public class UserProfile
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public bool Active { get; set; }

       
    }

    public class OrderItem
    {
        public string OrderNumber { get; set; }

        public DateTime OrderDate { get; set; }

        public string ItemName { get; set; }

        public string Status { get; set; }

        public decimal Price { get; set; }
    }
}
