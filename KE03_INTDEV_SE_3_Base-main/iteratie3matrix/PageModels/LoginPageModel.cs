using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using iteratie3matrix.Services;

#if ANDROID
using Microsoft.Maui.ApplicationModel;
using iteratie3matrix;
#endif

namespace iteratie3matrix.PageModels;

public partial class LoginPageModel : ObservableObject
{
    // WHAT: Service that waits for and returns NFC scan results.
    // WHY: Separates UI logic from Android-specific NFC code.
    private readonly NfcService _nfcService;

    public LoginPageModel(NfcService nfcService)
    {
        // WHAT: Store the injected NFC service.
        // HOW: Dependency Injection supplies a shared instance.
        // WHY: Allows communication with MainActivity.
        _nfcService = nfcService;
    }

    // =====================
    // NFC LOGIN
    // =====================

    [RelayCommand]
    private async Task ScanNfc()
    {
        try
        {
#if ANDROID
            // WHAT: Start the Android NFC reader.
            // HOW: Call MainActivity to enable ReaderMode.
            // WHY: Only scan while the user requests it.
            var activity = Platform.CurrentActivity as MainActivity;
            activity?.StartNfcSession();
#endif

            // WHAT: Inform the user that scanning has started.
            // WHY: ReaderMode only begins after the dialog closes.
            await Shell.Current.DisplayAlert(
                "Scanner actief",
                "De NFC-scanner is geactiveerd.\n\nHoud uw medewerkerspas tegen de achterkant van het toestel.\n\n(klik op OK en dan pas scannen, dit is een WIP)",
                "OK");

            // WHAT: Wait until MainActivity detects an NFC tag.
            // HOW: NfcService completes this task when a UID is received.
            // WHY: Keeps the UI responsive while scanning.
            var tagId = await _nfcService.StartScanAsync();

            // WHAT: Ignore empty scan results.
            if (string.IsNullOrWhiteSpace(tagId))
                return;

            // WHAT: Notify the user that the scan succeeded.
            // WHY: Confirms the employee card was detected.
            await Shell.Current.DisplayAlert(
                "Inloggen geslaagd",
                $"Medewerkerspas gedetecteerd.\n\nTag-ID: {tagId}",
                "OK");
        }
        catch (Exception ex)
        {
            // WHAT: Display unexpected NFC errors.
            // WHY: Prevents the application from failing silently.
            await Shell.Current.DisplayAlert(
                "Fout",
                ex.Message,
                "OK");
        }
    }

    // =====================
    // MANUAL LOGIN
    // =====================

    [RelayCommand]
    private async Task ManualLogin()
    {
        // WHAT: Open the manual login screen.
        // WHY: Fallback when NFC is unavailable.
        await Shell.Current.GoToAsync("//LoginPage");
    }

    // =====================
    // SUPPORT
    // =====================

    [RelayCommand]
    private async Task CantLogin()
    {
        // WHAT: Open the support page.
        // WHY: Gives the user assistance when login fails.
        await Shell.Current.GoToAsync("//SupportPage");
    }
}