using KE03_INTDEV_SE_2_Base.Models;

namespace KE03_INTDEV_SE_2_Base.Models.ViewModels;

public class CreateDeliveryViewModel
{
    public int? DeliveryId { get; set; }

    public bool IsEditMode { get; set; }

    public DateTime ToBeSentDate { get; set; } = DateTime.Today.AddDays(1);

    public List<Order> NewOrders { get; set; } = new();

    public List<Vehicle> Vehicles { get; set; } = new();

    public List<int> SelectedOrderIds { get; set; } = new();

    public int SelectedVehicleId { get; set; }

    public List<CreateDeliveryProductLineViewModel> ProductLines { get; set; } = new();
}

public class CreateDeliveryProductLineViewModel
{
    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string PackageDimension { get; set; } = string.Empty;

    public int AvailableQuantity { get; set; }

    public int SelectedQuantity { get; set; }
}