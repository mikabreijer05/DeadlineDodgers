using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Font = Microsoft.Maui.Font;

namespace iteratie3matrix;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        /*
            WHAT

            Registers detail page routes.

            WHY

            OrderListPage navigates using:

                Shell.Current.GoToAsync("order?id=...")

            Shell must know what "order" means.

            HOW

            Maps route name "order"
            to OrderDetailPage.
        */
        Routing.RegisterRoute(
            "order",
            typeof(Pages.OrderDetailPage));

        var theme = Application.Current!.RequestedTheme;
    }

    /*
        WHAT

        Displays a mobile toast message.

        WHY

        Used for lightweight notifications.

        HOW

        Uses CommunityToolkit toast service.
    */
    public static async Task DisplayToastAsync(string message)
    {
        if (OperatingSystem.IsWindows())
            return;

        var toast = Toast.Make(
            message,
            textSize: 18);

        var cts =
            new CancellationTokenSource(
                TimeSpan.FromSeconds(3));

        await toast.Show(cts.Token);
    }

    /*
        WHAT

        Switches between light and dark mode.

        WHY

        Allows user theme selection.

        HOW

        Uses MAUI AppTheme.
    */
    private void SfSegmentedControl_SelectionChanged(
        object sender,
        Syncfusion.Maui.Toolkit.SegmentedControl.SelectionChangedEventArgs e)
    {
        Application.Current!.UserAppTheme =
            e.NewIndex == 0
                ? AppTheme.Light
                : AppTheme.Dark;
    }
}