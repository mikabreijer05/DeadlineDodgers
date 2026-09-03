using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Nfc;
using Android.Nfc.Tech;
using Android.OS;
using iteratie3matrix.Services;

namespace iteratie3matrix;

/// <summary>
/// WHAT:
/// Android-specific entry point for the MAUI application.
///
/// WHY:
/// NFC Reader Mode is only available through Android's native API and
/// cannot be fully implemented using platform-independent .NET MAUI code.
///
/// HOW:
/// The project initially used Android's Foreground Dispatch (Intent-based)
/// implementation in an attempt to keep the NFC functionality as close to
/// standard MAUI as possible. During testing with a Dutch OV-chipkaart,
/// the Intent approach did not reliably return the scanned NFC tag.
/// The implementation was therefore migrated to Android Reader Mode,
/// which directly detects NFC tags and forwards the scanned UID to the
/// shared NfcService.
/// </summary>
[Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges =
        ConfigChanges.ScreenSize |
        ConfigChanges.Orientation |
        ConfigChanges.UiMode |
        ConfigChanges.ScreenLayout |
        ConfigChanges.SmallestScreenSize |
        ConfigChanges.Density
)]

/*
 * Previous Foreground Dispatch implementation.
 * This approach has been replaced by Reader Mode after reliability
 * issues were encountered during testing.
 *
 * [IntentFilter(new[] { NfcAdapter.ActionTagDiscovered })]
 * [IntentFilter(new[] { NfcAdapter.ActionNdefDiscovered })]
 * [IntentFilter(new[] { NfcAdapter.ActionTechDiscovered })]
 */
public class MainActivity : MauiAppCompatActivity, NfcAdapter.IReaderCallback
{
    /// <summary>
    /// Service used to return scanned NFC data to the shared MAUI application.
    /// </summary>
    private NfcService? _nfcService;

    /// <summary>
    /// Android NFC hardware adapter.
    /// </summary>
    private NfcAdapter? _nfcAdapter;

    /*
     * Previous Foreground Dispatch implementation.
     * Kept temporarily as development reference.
     *
     * private PendingIntent? _pendingIntent;
     */

    /// <summary>
    /// Indicates whether an NFC scan session is currently active.
    /// </summary>
    private bool _nfcSessionActive;

    /// <summary>
    /// WHAT:
    /// Initializes the Android NFC components.
    ///
    /// WHY:
    /// The application requires access to the device's NFC hardware
    /// before Reader Mode can be enabled.
    ///
    /// HOW:
    /// Retrieves both the shared NfcService and the Android NFC adapter.
    /// The previous Foreground Dispatch setup has been retained below as
    /// commented reference code.
    /// </summary>
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        _nfcService = IPlatformApplication.Current?.Services.GetService<NfcService>();
        _nfcAdapter = NfcAdapter.GetDefaultAdapter(this);

        /*
         * Previous Foreground Dispatch setup.
         *
         * var intent = new Intent(this, typeof(MainActivity))
         *     .AddFlags(ActivityFlags.SingleTop);
         *
         * _pendingIntent = PendingIntent.GetActivity(
         *     this,
         *     0,
         *     intent,
         *     PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable
         * );
         */
    }

    /// <summary>
    /// WHAT:
    /// Starts a new NFC scanning session.
    ///
    /// WHY:
    /// Reader Mode should only be active while the user is intentionally
    /// scanning an employee card. This prevents unnecessary NFC callbacks
    /// during normal application use.
    ///
    /// HOW:
    /// Enables Android Reader Mode and registers this activity as the
    /// callback that receives detected NFC tags.
    /// </summary>
    public void StartNfcSession()
    {
        if (_nfcAdapter == null)
            return;

        _nfcSessionActive = true;

        _nfcAdapter.EnableReaderMode(
            this,
            this,
            NfcReaderFlags.NfcA |
            NfcReaderFlags.NfcB |
            NfcReaderFlags.NfcF |
            NfcReaderFlags.NfcV |
            NfcReaderFlags.NfcBarcode |
            NfcReaderFlags.SkipNdefCheck,
            null);

        System.Diagnostics.Debug.WriteLine("ReaderMode ENABLED");
    }

    /// <summary>
    /// WHAT:
    /// Stops the active NFC scanning session.
    ///
    /// WHY:
    /// Once a card has been scanned, Reader Mode is disabled to prevent
    /// duplicate scans and unnecessary battery usage.
    ///
    /// HOW:
    /// Disables Android Reader Mode and resets the active session flag.
    /// </summary>
    public void StopNfcSession()
    {
        if (_nfcAdapter == null)
            return;

        _nfcSessionActive = false;

        _nfcAdapter.DisableReaderMode(this);

        System.Diagnostics.Debug.WriteLine("ReaderMode DISABLED");
    }

    /// <summary>
    /// WHAT:
    /// Processes an NFC tag detected by Android Reader Mode.
    ///
    /// WHY:
    /// The application needs the unique identifier (UID) of the employee
    /// card so the login process can continue within the shared MAUI code.
    ///
    /// HOW:
    /// Retrieves the tag UID, forwards it to the shared NfcService and
    /// disables Reader Mode to complete the scan session.
    /// </summary>
    public void OnTagDiscovered(Tag? tag)
    {

        // Debugging output to trace the NFC tag discovery process.
        System.Diagnostics.Debug.WriteLine("");
        System.Diagnostics.Debug.WriteLine("===================================");
        System.Diagnostics.Debug.WriteLine("READER MODE CALLBACK");
        System.Diagnostics.Debug.WriteLine("===================================");



        // Check if the NFC session is active and if the tag is valid.
        if (!_nfcSessionActive)
        {
            System.Diagnostics.Debug.WriteLine("Session not active");
            return;
        }


        // Check if the tag is null to avoid processing an invalid tag.
        if (tag == null)
        {
            System.Diagnostics.Debug.WriteLine("Tag == NULL");
            return;
        }


        // Attempt to retrieve the tag UID and handle any exceptions that may occur.
        try
        {
            var idBytes = tag.GetId();

            if (idBytes == null || idBytes.Length == 0)
            {
                System.Diagnostics.Debug.WriteLine("UID EMPTY");
                return;
            }

            var uid = BitConverter.ToString(idBytes);

            System.Diagnostics.Debug.WriteLine($"UID = {uid}");

            _nfcService?.SetResult(uid);

            RunOnUiThread(() =>
            {
                StopNfcSession();
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
        }
    }

    /*
     * Previous Foreground Dispatch implementation.
     *
     * Reader Mode replaces this implementation because it proved more
     * reliable when testing with Dutch OV-chipkaarten.
     * This code is retained as documentation of the development process
     * and can be removed once Reader Mode has been fully validated.
     *
     * protected override void OnNewIntent(Intent intent)
     * {
     *     ...
     * }
     */
}