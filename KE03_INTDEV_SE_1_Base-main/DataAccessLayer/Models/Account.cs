namespace DataAccessLayer.Models;

public class Account
{
    public int Id { get; set; }
        
    public string Name { get; set; }
        
    public string Address { get; set; }

    public bool Active { get; set; }
}