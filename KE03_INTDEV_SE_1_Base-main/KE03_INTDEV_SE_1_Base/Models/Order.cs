using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace KE03_INTDEV_SE_1_Base.Models;
public class Order
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public int? AddressId { get; set; }
    public Address? address { get; set; }
    

    [Required]
    public int CustomerId { get; set; }

    public Customer Customer { get; set; } = null!;

    // Initialize collections with empty lists to allow modifications
    public ICollection<OrderLine> OrderLines { get; private set; } = new List<OrderLine>();
    public ICollection<Product> Products { get; private set; } = new List<Product>();

    public Order()
    {
        // Constructor to initialize collections
    }

    // Method to add an order line
    public void AddOrderLine(OrderLine orderLine)
    {
        if (!OrderLines.Contains(orderLine))
        {
            OrderLines.Add(orderLine);
        }
    }

    // Method to add a product
    public void AddProduct(Product product)
    {
        if (!Products.Contains(product))
        {
            Products.Add(product);
        }
    }
}
