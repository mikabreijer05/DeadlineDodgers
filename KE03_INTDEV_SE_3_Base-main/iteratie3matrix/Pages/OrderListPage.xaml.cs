using iteratie3matrix.Models;
using iteratie3matrix.PageModels;

namespace iteratie3matrix.Pages;

public partial class OrderListPage : ContentPage
{
    // WHAT:
    // Reference to the PageModel.
    //
    // WHY:
    // Allows page lifecycle methods
    // to call ViewModel methods.
    private readonly OrderListPageModel _vm;

    public OrderListPage(OrderListPageModel vm)
    {
        // WHAT:
        // Load XAML UI.
        InitializeComponent();

        // WHAT:
        // Connect ViewModel.
        BindingContext = vm;

        _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // WHAT:
        // Refresh orders whenever page opens.
        //
        // WHY:
        // Ensures latest database data.
        await _vm.LoadAsync();
    }

    private async void CollectionView_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        // WHAT:
        // Gets selected order.
        var order =
            e.CurrentSelection.FirstOrDefault()
            as OrderListItem;

        if (order == null)
            return;

        //// DEBUG
        //await DisplayAlert(
        //    "Selected",
        //    $"Order {order.OrderId}",
        //    "OK");

        // Navigate directly
        await Shell.Current.GoToAsync(
            $"order?id={order.OrderId}");

        // Clear selection
        ((CollectionView)sender).SelectedItem = null;
    }
}