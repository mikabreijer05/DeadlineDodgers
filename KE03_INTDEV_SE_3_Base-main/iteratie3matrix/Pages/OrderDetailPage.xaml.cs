using iteratie3matrix.PageModels;

namespace iteratie3matrix.Pages;

public partial class OrderDetailPage : ContentPage
{
    public OrderDetailPage(OrderDetailPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}