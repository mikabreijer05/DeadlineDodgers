using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class AccountModel : PageModel
    {
        private readonly ICustomerRepository _customerRepository;

        public AccountModel(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
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

        public void OnGet(int? id)
        {
            if (id == null)
            {
                // fallback to first customer if no id supplied
                var first = _customerRepository.GetAllCustomers().FirstOrDefault();
                if (first == null) return;
                id = first.Id;
            }

            var customer = _customerRepository.GetCustomerById(id.Value);
            if (customer == null) return;

            UserData = new UserProfile
            {
                Name = customer.Name,
                Address = customer.Address,
                Active = customer.Active,
            };
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
