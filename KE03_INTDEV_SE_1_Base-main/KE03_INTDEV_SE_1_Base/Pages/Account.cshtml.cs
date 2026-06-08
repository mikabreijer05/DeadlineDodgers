using KE03_INTDEV_SE_1_Base.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class AccountModel : PageModel
    {
        private readonly SQLCustomer _customerService;
        private readonly SQLOrder _orderService;

        public AccountModel(SQLCustomer customerService, SQLOrder orderService)
        {
            _customerService = customerService;
            _orderService = orderService;
        }

        public IActionResult OnPostLogout()
        {
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

            var customer = _customerService.GetCustomerById(id.Value);
            if (customer == null)
            {
                return NotFound();
            }

            UserData = new UserProfile
            {
                Name = customer.Name,
                Street = customer.Address.Street,
                HouseNumber = customer.Address.HouseNumber,
                PostalCode = customer.Address.PostalCode,
                City = customer.Address.City,
                Country = customer.Address.Country,
                Active = customer.Active,
            };

            Orders = _orderService.GetOrdersByCustomerId(customer.Id)
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
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
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