using System.ComponentModel.DataAnnotations;

namespace KE03_INTDEV_SE_1_Base.Models;

public class Account
{
    public int Id { get; set; }
        
    [Required(ErrorMessage = "Naam is verplicht.")]
    public string Name { get; set; }

    public string? UserName { get; set; }
        
    public Address Address { get; set; }

    public bool Active { get; set; }
}