namespace KE03_INTDEV_SE_2_Base_main.Models;

public class CustomerOrder : Order
{
    public int CustomerOrderId { get; set; }
    public Customer Customer { get; set; }
}