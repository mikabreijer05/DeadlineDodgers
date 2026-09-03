namespace KE03_INTDEV_SE_2_Base.Models;

public class Review
{
    public int ReviewId { get; set; }

    public int ProductId { get; set; }

    // Product display name populated by DAL
    public string ProductName { get; set; }

    public string Username { get; set; }

    public int Rating { get; set; }

    public string Title { get; set; }

    public string Comment { get; set; }

    public string ReviewStatus { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public bool IsVerified { get; set; } = false;


    public Product Product { get; set; }
}
