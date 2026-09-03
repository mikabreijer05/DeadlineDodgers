public class CartItem
{
    public int ProductId { get; set; }
    public int RequiredQuantity { get; set; }
    public int Remaining { get; set; }
    public string? ProductName { get; set; }
    public decimal? Price { get; set; }
}