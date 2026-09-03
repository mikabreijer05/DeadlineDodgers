namespace iteratie3matrix.Models;

public class OrderListItem
{
    // =========================
    // DELIVERY INFORMATION
    // =========================

    /*
        WHAT:
        Delivery identifier.

        WHY:
        Allows the courier to identify the delivery.
    */
    public int OrderId { get; set; }

    /*
        WHAT:
        Date of the order/delivery.

        WHY:
        Used for sorting deliveries.
    */
    public DateTime OrderDate { get; set; }

    /*

    */
    public int StatusId { get; set; }

    /*
    */
    public string StatusName { get; set; } = "";

    /*
        WHAT:
        Display date.

        WHY:
        Keeps formatting out of XAML.
    */
    public string DeliveryDate =>
        OrderDate.ToString("dd-MM-yyyy");


    public string DeliveryLabel =>
        $"Delivery #{OrderId}";
}