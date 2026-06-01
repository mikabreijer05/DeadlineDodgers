namespace KE03_INTDEV_SE_2_Base_main.Models;

public class CustomerDelivery : Delivery
{
    public int CustomerId { get; set; }
    public Customer Customer { get; set; }
}