using KE03_INTDEV_SE_2_Base.Models;

namespace KE03_INTDEV_SE_2_Base.Models;

public class CustomerOrder : Order
{
    public int CustomerOrderId { get; set; }
    public Customer Customer { get; set; }
}