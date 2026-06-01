namespace DataAccessLayer.Models;

public class CustomerDelivery : Delivery
{
    public int CustomerId { get; set; }
    public Customer Customer { get; set; }
}