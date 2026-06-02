using KE03_INTDEV_SE_2_Base.Models;

namespace KE03_INTDEV_SE_2_Base_main.Models;

public class DistributorOrder : Order
{
    public int DistributorId { get; set; }
    public Distributor Distributor { get; set; }
}