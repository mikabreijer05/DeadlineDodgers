using KE03_INTDEV_SE_2_Base.Models;

namespace KE03_INTDEV_SE_2_Base.Models;

public class Delivery
{
    public int Id { get; set; }
    public DateTime ToBeSentDate { get; set; }
    public List<OrderLine> ProductLines { get; set; }
    public Vehicle Vehicle { get; set; }
}