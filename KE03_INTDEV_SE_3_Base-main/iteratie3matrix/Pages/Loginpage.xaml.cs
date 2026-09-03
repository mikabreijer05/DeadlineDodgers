namespace iteratie3matrix.Pages;

public partial class Loginpage : ContentPage
{
    public Loginpage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object? sender, EventArgs e)
    {
        var username = this.FindByName<Entry>("UsernameEntry")?.Text?.Trim();
        var password = this.FindByName<Entry>("PasswordEntry")?.Text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Error", "Enter username and password", "OK");
            return;
        }

        // TODO: Replace with real authentication
        if (username == "user" && password == "password")
        {
            // Navigate to the VanSelectionPage
            await Shell.Current.GoToAsync("//vans");
        }
        else
        {
            await DisplayAlert("Error", "Invalid credentials", "OK");
        }
    }

    private async void OnCantLoginClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//support");
    }
}
