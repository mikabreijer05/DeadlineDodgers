namespace DataAccessLayer.Models;

public class DistributorOrder : Order
{
    public int DistributorId { get; set; }
    public Distributor Distributor { get; set; }
}