namespace KE03_INTDEV_SE_1_Base.Models;

public class ProductList
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Product> Products { get; set; }
}