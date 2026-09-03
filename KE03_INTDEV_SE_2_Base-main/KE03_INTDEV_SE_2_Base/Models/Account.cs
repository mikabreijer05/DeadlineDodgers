namespace KE03_INTDEV_SE_2_Base.Models;

public class Account
{
    public int Id { get; set; }
        
    public string Name { get; set; }
        
    public int AddressId { get; set; }
    public Address Address { get; set; }

    public bool Active { get; set; }
}