using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class LoginModel : PageModel
    {
        private readonly DataAccessLayer.Interfaces.ICustomerRepository _customerRepository;

        public LoginModel(DataAccessLayer.Interfaces.ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public IEnumerable<DataAccessLayer.Models.Customer> Customers { get; set; } = Enumerable.Empty<DataAccessLayer.Models.Customer>();

        [BindProperty]
        public int SelectedCustomerId { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
            Customers = _customerRepository.GetAllCustomers();
        }

        public IActionResult OnPost()
        {
            if (SelectedCustomerId > 0)
            {
                // Set cookie to remember logged in user
                var customer = _customerRepository.GetCustomerById(SelectedCustomerId);
                if (customer != null)
                {
                    var options = new Microsoft.AspNetCore.Http.CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddDays(7),
                        HttpOnly = true,
                    };
                    Response.Cookies.Append("LoggedInCustomerId", customer.Id.ToString(), options);
                    Response.Cookies.Append("LoggedInCustomerName", customer.Name, options);
                }

                // Redirect to account page with selected customer id
                return RedirectToPage("/Account", new { id = SelectedCustomerId });
            }

            ErrorMessage = "Selecteer een gebruiker om in te loggen";
            Customers = _customerRepository.GetAllCustomers();
            return Page();
        }
    }
}