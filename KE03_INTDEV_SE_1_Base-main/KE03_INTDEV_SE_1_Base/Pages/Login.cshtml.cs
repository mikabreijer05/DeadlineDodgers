using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KE03_INTDEV_SE_1_Base.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Gebruikersnaam { get; set; }

        [BindProperty]
        public string Wachtwoord { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            // Simpele check
            if (Gebruikersnaam == "admin" && Wachtwoord == "1234")
            {
                // Succesvol ingelogd
                return RedirectToPage("/Account");
            }

            ErrorMessage = "Ongeldige gebruikersnaam of wachtwoord";
            return Page();
        }
    }
}