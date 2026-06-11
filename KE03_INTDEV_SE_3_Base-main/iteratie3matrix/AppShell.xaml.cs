using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Font = Microsoft.Maui.Font;

namespace iteratie3matrix;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        var theme = Application.Current!.RequestedTheme;
        ThemeSegmentedControl.SelectedIndex = theme == AppTheme.Light ? 0 : 1;
    }

    public static async Task DisplayToastAsync(string message)
    {
        if (OperatingSystem.IsWindows())
            return;

        var toast = Toast.Make(message, textSize: 18);
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
        await toast.Show(cts.Token);
    }

    private void SfSegmentedControl_SelectionChanged(object sender, Syncfusion.Maui.Toolkit.SegmentedControl.SelectionChangedEventArgs e)
    {
        Application.Current!.UserAppTheme = e.NewIndex == 0 ? AppTheme.Light : AppTheme.Dark;
    }
}