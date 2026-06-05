using System.ComponentModel.DataAnnotations;
using KE03_INTDEV_SE_2_Base_main.Models;

namespace KE03_INTDEV_SE_2_Base.Models;

public class OrderLine
{
    [Required]
    public int OrderId { get; set; }

    public Order Order { get; set; } = null!;

    [Required]
    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;

    [Required]
    public int Quantity { get; set; }

    // Additional properties for display (from Product table)
    public string? ProdName { get; set; }

    public decimal? ProdPrice { get; set; }
}