using iteratie3matrix.PageModels;

namespace iteratie3matrix.Pages;

public partial class OrderListPage : ContentPage
{
    public OrderListPage(OrderListPageModel pageModel)
    {
        InitializeComponent();
        BindingContext = pageModel;
    }
}