using iteratie3matrix.PageModels;

namespace iteratie3matrix.Pages;

public partial class VanSelectionPage : ContentPage
{
    /*
        WHAT

        Stores PageModel reference.

        WHY

        Allows lifecycle methods
        to call LoadAsync().
    */
    private readonly VanSelectionPageModel _vm;

    public VanSelectionPage(
        VanSelectionPageModel vm)
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