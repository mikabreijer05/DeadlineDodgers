using iteratie3matrix.PageModels;

namespace iteratie3matrix.Pages;

public partial class VehicleInspectionPage : ContentPage
{
    /*
        WHAT:
        Code-behind constructor.

        HOW:
        Receives PageModel via Dependency Injection.

        WHY:
        Keeps UI logic separated from business logic (MVVM pattern).
    */
    public VehicleInspectionPage(VehicleInspectionPageModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}