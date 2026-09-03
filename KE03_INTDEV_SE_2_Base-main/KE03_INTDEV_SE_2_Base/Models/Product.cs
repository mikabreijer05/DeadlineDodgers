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
        
        public ICollection<Order> Orders { get; } = new List<Order>();
        // Category relation
        public int CategoryId { get; set; }

        // Extra fields from SQL table
        public int Quantity { get; set; }
        public int? RemainingQuantity { get; set; }

        public int? ProductQuantity { get; set; }
        public string DeliveryTime { get; set; }
        public int? DiscountId { get; set; }
        public decimal Cost { get; set; }
        public string? Dimensions { get; set; }
        public int? DimensionId { get; set; }
    }
}