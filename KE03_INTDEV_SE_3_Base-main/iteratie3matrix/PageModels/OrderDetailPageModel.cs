using CommunityToolkit.Mvvm.ComponentModel;
using iteratie3matrix.Data;
using iteratie3matrix.Models;

namespace iteratie3matrix.PageModels;

public partial class OrderDetailPageModel : ObservableObject, IQueryAttributable
{
    private readonly OrderRepository _orderRepository;

    private Order? _order;

    [ObservableProperty]
    private int orderId;

    [ObservableProperty]
    private DateTime orderDate;

    [ObservableProperty]
    private int statusId;

    [ObservableProperty]
    private int accountId;

    [ObservableProperty]
    private int addressId;

    [ObservableProperty]
    private bool deliveryTogether;

    public OrderDetailPageModel(OrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.ContainsKey("id"))
            return;

        var id = Convert.ToInt32(query["id"]);

        var order = (await _orderRepository.ListAsync())
            .FirstOrDefault(o => o.OrderId == id);

        if (order is null)
            return;

        _order = order;

        OrderId = order.OrderId;
        OrderDate = order.OrderDate;
        StatusId = order.StatusId;
        AccountId = order.AccountId;
        AddressId = order.AddressId;
        DeliveryTogether = order.DeliveryTogether;
    }
}