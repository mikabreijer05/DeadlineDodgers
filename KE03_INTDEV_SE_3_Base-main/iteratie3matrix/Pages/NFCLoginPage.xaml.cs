using iteratie3matrix.PageModels;

namespace iteratie3matrix.Pages;

// WHAT:
// NFC login page.
// WHY:
// Displays the UI for employee card login.
// HOW:
// Uses MVVM by receiving its ViewModel through dependency injection.
public partial class NFCLoginPage : ContentPage
{
    // WHAT: ViewModel instance for this page.
    // WHY: Handles NFC scanning and navigation logic.
    private readonly LoginPageModel _vm;

    public NFCLoginPage(LoginPageModel vm)
    {
        InitializeComponent();

        // WHAT: Connect the ViewModel to the XAML page.
        // WHY: Enables Command and property bindings.
        // HOW: Assign the injected ViewModel as the BindingContext.
        BindingContext = vm;

        _vm = vm;
    }
}