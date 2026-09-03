using iteratie3matrix.PageModels;

namespace iteratie3matrix.Pages;

public partial class CartScannerPage : ContentPage
{
    private readonly CartScannerPageModel _vm;

    public CartScannerPage(CartScannerPageModel vm)
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