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

    // IMPORTANT: selection trigger
    [ObservableProperty]
    private Order? selectedOrder;

    public OrderListPageModel(OrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task LoadAsync()
    {
        Orders = await _orderRepository.ListAsync();
    }

    partial void OnSelectedOrderChanged(Order? value)
    {
        if (value is null)
            return;

        Navigate(value);
    }

    private async void Navigate(Order order)
    {
        await Shell.Current.GoToAsync($"order?id={order.OrderId}");
        SelectedOrder = null; // reset selection so you can tap again
    }
}