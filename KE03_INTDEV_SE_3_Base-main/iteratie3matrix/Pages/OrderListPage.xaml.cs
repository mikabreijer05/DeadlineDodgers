using iteratie3matrix.PageModels;

namespace iteratie3matrix.Pages;

public partial class OrderListPage : ContentPage
{
    private readonly OrderListPageModel _vm;

    public OrderListPage(OrderListPageModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadAsync();
    }
}