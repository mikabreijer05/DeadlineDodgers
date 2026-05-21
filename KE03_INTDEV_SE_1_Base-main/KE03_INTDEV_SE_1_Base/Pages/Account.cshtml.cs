using DataAccessLayer.Interfaces;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class AccountModel : PageModel
    {
        private readonly ICustomerRepository _customerRepository;
        public UserProfile UserData { get; set; }

        public List<OrderItem> Orders { get; set; }

        public void OnGet()
        {
            Customer customer = @_customerRepository.GetCustomerById(1);
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
