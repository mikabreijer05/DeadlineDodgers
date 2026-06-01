namespace DataAccessLayer.Models;

public class CustomerOrder : Order
{
    public int CustomerOrderId { get; set; }
    public Customer Customer { get; set; }
}