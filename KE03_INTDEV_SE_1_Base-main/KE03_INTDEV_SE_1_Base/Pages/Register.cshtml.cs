using System.Text.Json;
using KE03_INTDEV_SE_1_Base.DAL;
using KE03_INTDEV_SE_1_Base.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KE03_INTDEV_SE_1_Base.Pages;

public class Register : PageModel
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly SQLCustomer _customerService;
    public Register(HttpClient httpClient, IConfiguration configuration, SQLCustomer customerService)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _customerService = customerService;
    }

    public List<SelectListItem> CountryOptions { get; set; } = new();

    public void OnGet()
    {
        Account.Address ??= new Address();
        LoadCountryOptions();
    }

    public string ErrorMessage { get; set; }

    [BindProperty]
    public Account Account { get; set; } = new()
    {
        Address = new Address()
    };

    public async Task<IActionResult> OnPostAsync()
    {
        Account.Address ??= new Address();
        LoadCountryOptions();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var addressIsValid = await ValidateAddressWithGeoapifyAsync();

        if (!addressIsValid)
        {
            return Page();
        }

        var customer = new Customer
        {
            UserName = Account.UserName,
            Name = Account.Name,
            Active = true,
            Address = Account.Address
        };

        _customerService.AddCustomer(customer);

        Response.Cookies.Append("LoggedInCustomerId", customer.Id.ToString());
        Response.Cookies.Append("LoggedInCustomerName", customer.Name);

        return RedirectToPage("/Account", new { id = customer.Id });
    }

    private void LoadCountryOptions()
    {
        CountryOptions =
        [
            new SelectListItem { Value = "", Text = "-- Kies een land --" },
            new SelectListItem { Value = "Netherlands", Text = "Nederland" },
            new SelectListItem { Value = "Belgium", Text = "België" },
            new SelectListItem { Value = "Germany", Text = "Duitsland" },
            new SelectListItem { Value = "France", Text = "Frankrijk" },
            new SelectListItem { Value = "United Kingdom", Text = "Verenigd Koninkrijk" },
            new SelectListItem { Value = "United States", Text = "Verenigde Staten" }
        ];
    }

    private async Task<bool> ValidateAddressWithGeoapifyAsync()
    {
        var apiKey = _configuration["Geoapify:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            ErrorMessage = "Adresvalidatie is tijdelijk niet beschikbaar. De API-sleutel ontbreekt.";
            ModelState.AddModelError(string.Empty, ErrorMessage);
            return false;
        }

        var address = Account.Address;

        var fullAddress = $"{address.Street} {address.HouseNumber}, {address.PostalCode} {address.City}, {address.Country}";
        var encodedAddress = Uri.EscapeDataString(fullAddress);

        var requestUrl =
            $"https://api.geoapify.com/v1/geocode/search?text={encodedAddress}&apiKey={apiKey}";

        try
        {
            using var response = await _httpClient.GetAsync(requestUrl);

            if (!response.IsSuccessStatusCode)
            {
                ErrorMessage = "Adresvalidatie is mislukt. Probeer het later opnieuw.";
                ModelState.AddModelError(string.Empty, ErrorMessage);
                return false;
            }

            var json = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            if (!root.TryGetProperty("features", out var features) || features.GetArrayLength() == 0)
            {
                ErrorMessage = "Het opgegeven adres kon niet worden gevonden. Controleer je adresgegevens.";
                ModelState.AddModelError("Account.Address.Street", "Controleer de straatnaam.");
                ModelState.AddModelError("Account.Address.HouseNumber", "Controleer het huisnummer.");
                ModelState.AddModelError("Account.Address.PostalCode", "Controleer de postcode.");
                ModelState.AddModelError("Account.Address.City", "Controleer de stad.");
                ModelState.AddModelError("Account.Address.Country", "Controleer het land.");
                return false;
            }

            var firstResult = features[0];

            if (!firstResult.TryGetProperty("properties", out var properties))
            {
                ErrorMessage = "Het adres kon niet correct worden gevalideerd.";
                ModelState.AddModelError(string.Empty, ErrorMessage);
                return false;
            }

            var rankConfidence = 0.0;

            if (properties.TryGetProperty("rank", out var rank) &&
                rank.TryGetProperty("confidence", out var confidenceElement))
            {
                rankConfidence = confidenceElement.GetDouble();
            }

            // Confidence must be high enough
if (rankConfidence < 0.95)
{
    ErrorMessage = "Het opgegeven adres lijkt niet betrouwbaar genoeg.";
    ModelState.AddModelError("Account.Address.Street", "Controleer de straatnaam.");
    ModelState.AddModelError("Account.Address.HouseNumber", "Controleer het huisnummer.");
    ModelState.AddModelError("Account.Address.PostalCode", "Controleer de postcode.");
    ModelState.AddModelError("Account.Address.City", "Controleer de stad.");
    ModelState.AddModelError("Account.Address.Country", "Controleer het land.");
    return false;
}

// Read the returned address components
var returnedStreet = properties.TryGetProperty("street", out var streetElement)
    ? streetElement.GetString() ?? string.Empty
    : string.Empty;

var returnedHouseNumber = properties.TryGetProperty("housenumber", out var houseNumberElement)
    ? houseNumberElement.GetString() ?? string.Empty
    : string.Empty;

var returnedCity = properties.TryGetProperty("city", out var cityElement)
    ? cityElement.GetString() ?? string.Empty
    : string.Empty;

var returnedPostcode = properties.TryGetProperty("postcode", out var postcodeElement)
    ? postcodeElement.GetString() ?? string.Empty
    : string.Empty;

var returnedCountry = properties.TryGetProperty("country", out var countryElement)
    ? countryElement.GetString() ?? string.Empty
    : string.Empty;

// Compare each component
            if (!EqualsNormalized(returnedStreet, address.Street) ||
                !EqualsNormalized(returnedHouseNumber, address.HouseNumber) ||
                !EqualsNormalized(returnedCity, address.City) ||
                !EqualsNormalized(returnedPostcode, address.PostalCode) ||
                !EqualsNormalized(returnedCountry, address.Country))
            {
                ErrorMessage = "Het opgegeven adres komt niet exact overeen met een bestaand adres.";
                ModelState.AddModelError("Account.Address.Street", "Controleer de straatnaam.");
                ModelState.AddModelError("Account.Address.HouseNumber", "Controleer het huisnummer.");
                ModelState.AddModelError("Account.Address.PostalCode", "Controleer de postcode.");
                ModelState.AddModelError("Account.Address.City", "Controleer de stad.");
                ModelState.AddModelError("Account.Address.Country", "Controleer het land.");
                return false;
            }

            return true;
        }
        catch (HttpRequestException)
        {
            ErrorMessage = "Er kon geen verbinding worden gemaakt met de adresvalidatieservice.";
            ModelState.AddModelError(string.Empty, ErrorMessage);
            return false;
        }
        catch (JsonException)
        {
            ErrorMessage = "De adresvalidatieservice gaf een ongeldig antwoord terug.";
            ModelState.AddModelError(string.Empty, ErrorMessage);
            return false;
        }
        catch (TaskCanceledException)
        {
            ErrorMessage = "Adresvalidatie duurde te lang. Probeer het opnieuw.";
            ModelState.AddModelError(string.Empty, ErrorMessage);
            return false;
        }
    }
    private static bool EqualsNormalized(string? value1, string? value2)
    {
        static string Normalize(string? value) =>
            (value ?? string.Empty)
            .Trim()
            .Replace(" ", "")
            .ToUpperInvariant();

        return Normalize(value1) == Normalize(value2);
    }
}