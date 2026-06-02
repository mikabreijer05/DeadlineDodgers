using KE03_INTDEV_SE_2_Base_main.Models;

namespace KE03_INTDEV_SE_2_Base.Models;

public class CustomerServiceTicket
{
    public int Id { get; set; }
    public DateTime DateCreated { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; }
    public bool IsActive { get; set; }
    public List<ContactDescriptions> ContactDescriptions { get; set; } = new List<ContactDescriptions>();
}