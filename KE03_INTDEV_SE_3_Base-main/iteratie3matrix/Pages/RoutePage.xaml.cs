namespace iteratie3matrix.Pages;

public partial class RoutePage : ContentPage
{
	public RoutePage()
	{
		InitializeComponent();
	}

    private async void OnRouteClicked(object? sender, EventArgs e)
	{
        // Navigate to the Route Page
        await Launcher.Default.OpenAsync("https://www.google.com/maps/dir/Zuyd+Hogeschool+-+locatie+Heerlen,+Nieuw+Eyckholt+300,+6419+DJ+Heerlen/Geleenstraat+25,+6411+HP+Heerlen/@50.8836626,5.9633299,16z/data=!3m1!4b1!4m14!4m13!1m5!1m1!1s0x47c0bdc055ab9379:0x8425bd9cdc8235a5!2m2!1d5.9588567!2d50.8813976!1m5!1m1!1s0x47c0bdbb136ef6f9:0x42256810f04fe952!2m2!1d5.9781242!2d50.8868921!3e0?entry=ttu&g_ep=EgoyMDI2MDYyNC4wIKXMDSoASAFQAw%3D%3D");
    }
}