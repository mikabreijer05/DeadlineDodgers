using System.ComponentModel.DataAnnotations;

namespace KE03_INTDEV_SE_1_Base.Models;

public class Address
{
    public int AddressId { get; set; }

    [Required(ErrorMessage = "Straat is verplicht.")]
    public string Street { get; set; }

    [Required(ErrorMessage = "Huisnummer is verplicht.")]
    public string HouseNumber { get; set; }

    [Required(ErrorMessage = "Postcode is verplicht.")]
    public string PostalCode { get; set; }

    [Required(ErrorMessage = "Stad is verplicht.")]
    public string City { get; set; }

    [Required(ErrorMessage = "Land is verplicht.")]
    public string Country { get; set; }
}