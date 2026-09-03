namespace iteratie3matrix.Models;

/// <summary>
/// Cart used during loading process.
/// Driver scans packages into this cart.
/// </summary>
public class CartLoad
{
    public string CartNumber { get; set; } = string.Empty;

    public int ExpectedPackages { get; set; }

    public int ScannedPackages { get; set; }

    public List<CartItem> Items { get; set; } = new();
}