
namespace KE03_INTDEV_SE_1_Base.Models;

public class DistributorOrder : Order
{
    public int DistributorId { get; set; }
    public Distributor Distributor { get; set; }
}