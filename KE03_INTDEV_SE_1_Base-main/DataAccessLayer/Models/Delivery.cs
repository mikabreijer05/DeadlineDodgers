namespace DataAccessLayer.Models;

public class Delivery
{
    public int Id { get; set; }
    public string OrderId { get; set; }
    public Order Order { get; set; }
    public string DeliveryMethod { get; set; }
    public bool Sent { get; set; }
    public DateTime SentTime { get; set; }
    public bool Processed { get; set; }
    public DateTime ProcessedTime { get; set; }
    public bool ReadyForDelivery { get; set; }
    public DateTime ReadyForDeliveryTime { get; set; }
    public bool Delivered { get; set; }
    public DateTime DeliveredTime { get; set; }
}