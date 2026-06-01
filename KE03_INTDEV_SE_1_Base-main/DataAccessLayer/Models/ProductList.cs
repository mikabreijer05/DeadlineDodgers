namespace DataAccessLayer.Models;

public class ProductList
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Product> Products { get; set; }
}