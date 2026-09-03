using System.ComponentModel.DataAnnotations;

namespace KE03_INTDEV_SE_1_Base.Models;

public class OrderLine
{
    public int Id { get; set; }

    [Required]
    public int OrderId { get; set; }

    public Order Order { get; set; } = null!;

    [Required]
    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;

    [Required]
    public int Quantity { get; set; }

    [Required]
    public decimal PricePerProduct { get; set; }
}