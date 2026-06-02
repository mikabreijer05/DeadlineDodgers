using System.Collections;
using Azure;
using KE03_INTDEV_SE_1_Base.DAL;
using KE03_INTDEV_SE_1_Base.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class LoginModel : PageModel
    {
        private readonly SQLCustomer _customerService;

        public LoginModel(SQLCustomer customerService)
        {
            _customerService = customerService;
        }

        public IEnumerable<Customer> Customers { get; set; } = Enumerable.Empty<Customer>();

        [BindProperty] public int SelectedCustomerId { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
            Customers = _customerService.GetAllCustomers();
        }

        public IActionResult OnPost()
        {
            if (SelectedCustomerId > 0)
            {
                var customer = _customerService.GetCustomerById(SelectedCustomerId);
                if (customer != null)
                {
                    var options = new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddDays(7),
                        HttpOnly = true,
                    };
                    Response.Cookies.Append("LoggedInCustomerId", customer.Id.ToString(), options);
                    Response.Cookies.Append("LoggedInCustomerName", customer.Name, options);
                }

                return RedirectToPage("/Account", new { id = SelectedCustomerId });
            }

            ErrorMessage = "Selecteer een gebruiker om in te loggen";
            Customers = _customerService.GetAllCustomers();
            return Page();
        }
    }
}
