using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using iteratie3matrix.Data;
using iteratie3matrix.Models;

namespace iteratie3matrix.PageModels;

public partial class OrderListPageModel : ObservableObject
{
    private readonly OrderRepository _orderRepository;

    [ObservableProperty]
    private List<Order> orders = [];

    public OrderListPageModel(OrderRepository orderRepository)
    {
        _orderRepository = orderRepository;

        _ = Load();
    }

    private async Task Load()
    {
        Orders = await _orderRepository.ListAsync();
    }

    [RelayCommand]
    private async Task NavigateToOrder(Order order)
    {
        if (order is null)
            return;

        await AppShell.DisplayToastAsync($"Order {order.OrderId} selected");
    }
}