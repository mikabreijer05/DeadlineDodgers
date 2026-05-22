namespace DataAccessLayer.Models;

public class Review
{
    public int Id { get; set; }
    
    public int ProductId { get; set; }
    
    public string Username { get; set; }
    
    public int Rating { get; set; }
    
    public string Title { get; set; }
    
    public string Comment { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public bool IsVerified { get; set; } = false;
    
    
    public Product Product { get; set; }
}