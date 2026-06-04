namespace KE03_INTDEV_SE_2_Base.Models
{
    public class Product
    {
        // Primary key
        public int Id { get; set; }

        // Basic product info
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }

<<<<<<< Updated upstream
        public ICollection<Order> Orders { get; } = new List<Order>();
        public string? ProductDimensions { get; set; }
=======
        // Category relation
        public int CategoryId { get; set; }
>>>>>>> Stashed changes

        // Extra fields from SQL table
        public int Quantity { get; set; }
        public int DDCId { get; set; }
        public string DeliveryTime { get; set; }
        public int? DiscountId { get; set; }
        public decimal Cost { get; set; }
        public string Dimensions { get; set; }
    }
}