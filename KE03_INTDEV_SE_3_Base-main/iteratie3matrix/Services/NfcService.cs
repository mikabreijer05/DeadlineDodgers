namespace iteratie3matrix.Services;

/// <summary>
/// WHAT: Shares NFC scan results between Android native code and the MAUI ViewModel.
/// WHY: ReaderMode callbacks occur outside the UI layer and must return the scanned UID.
/// HOW: Uses a TaskCompletionSource to await a scan and buffers early scans until requested.
/// </summary>
public class NfcService
{
    // WHAT: Completes when a scan finishes.
    // WHY: Lets the ViewModel await an NFC scan asynchronously.
    private TaskCompletionSource<string>? _tcs;

    // WHAT: Temporary storage for an early scan.
    // WHY: Prevents scans from being lost if ReaderMode fires before awaiting starts.
    private string? _pendingResult;

    /// <summary>
    /// WHAT: Starts a new NFC scan session.
    /// WHY: Returns a Task that completes when a tag is detected.
    /// HOW: Creates a new TaskCompletionSource and immediately returns any buffered scan.
    /// </summary>
    public Task<string> StartScanAsync()
    {
        System.Diagnostics.Debug.WriteLine("NFC: StartScanAsync CREATED");

        _tcs = new TaskCompletionSource<string>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        // Return a buffered scan immediately if one already exists.
        if (!string.IsNullOrEmpty(_pendingResult))
        {
            var result = _pendingResult;
            _pendingResult = null;

            _tcs.TrySetResult(result);
        }

        return _tcs.Task;
    }

    /// <summary>
    /// WHAT: Stores the scanned NFC UID.
    /// WHY: Delivers the result to the waiting ViewModel.
    /// HOW: Completes the active Task or buffers the scan if none is waiting.
    /// </summary>
    public void SetResult(string tagData)
    {
        System.Diagnostics.Debug.WriteLine($"NFC: SetResult CALLED {tagData}");

        // Buffer scans that arrive before StartScanAsync().
        if (_tcs == null)
        {
            System.Diagnostics.Debug.WriteLine("NFC: buffered result (no active scan)");
            _pendingResult = tagData;
            return;
        }

        var tcs = _tcs;
        _tcs = null;

        tcs.TrySetResult(tagData);
    }
}