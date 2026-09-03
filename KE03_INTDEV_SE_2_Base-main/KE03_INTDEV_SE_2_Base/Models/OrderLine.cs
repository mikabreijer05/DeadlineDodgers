using System.ComponentModel.DataAnnotations;

namespace KE03_INTDEV_SE_2_Base.Models;

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

    public int? RemainingQuantity { get; set; }
    // Additional properties for display (from Product table)
    public string? ProdName { get; set; }

    public decimal? ProdPrice { get; set; }
    public int? PackageDimensionId { get; set; }
    public string? PackageDimension { get; set; }
    public string? OrderStatus { get; set; }
}