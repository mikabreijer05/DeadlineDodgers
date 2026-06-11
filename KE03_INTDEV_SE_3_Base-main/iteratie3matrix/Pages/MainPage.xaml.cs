using iteratie3matrix.Models;
using iteratie3matrix.PageModels;

namespace iteratie3matrix.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageModel model)
        {
            InitializeComponent();
            BindingContext = model;
        }
    }
}