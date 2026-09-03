namespace iteratie3matrix.Models;

public class VehicleDamageReport : ContentPage
{
	public VehicleDamageReport()
	{
		Content = new VerticalStackLayout
		{
			Children = {
				new Label { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, Text = "Welcome to .NET MAUI!"
				}
			}
		};
	}
}