namespace iteratie3matrix.Pages;

public partial class VehicleDamagePage : ContentPage
{
    public VehicleDamagePage(VehicleDamagePageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}