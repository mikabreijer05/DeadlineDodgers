using DataAccessLayer.Models;

namespace KE03_INTDEV_SE_2_Base_main.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }

        public ICollection<Order> Orders { get; } = new List<Order>();

        public ICollection<Review> Reviews { get; } = new List<Review>();
    }
}