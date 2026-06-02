namespace KE03_INTDEV_SE_2_Base.Models;

public class Distributor
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string ContactMailAddress { get; set; }
    public List<string> DeliveryMethods { get; set; }
}