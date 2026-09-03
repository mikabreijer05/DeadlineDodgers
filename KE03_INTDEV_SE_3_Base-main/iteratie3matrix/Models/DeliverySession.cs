namespace iteratie3matrix.Models;

public class DeliverySession
{
    // =========================
    // SHIFT DATA
    // =========================
    public Van? SelectedVan { get; set; }
    public bool ShiftStarted { get; set; }
    public bool HasInspectionPassed { get; set; }
    public bool IsLoading { get; set; }
    public bool IsCartComplete { get; set; }

    // =========================
    // LOGIN STATE 
    // =========================

    // WHAT:
    // Indicates if user has authenticated successfully
    // WHY:
    // Used to gate navigation to protected pages
    public bool IsLoggedIn { get; set; }

    // WHAT:
    // Stores identifier of logged-in courier/agent
    // WHY:
    // Used for logging, auditing, and session tracking
    public string? AgentId { get; set; }

    // OPTIONAL: raw tag payload for debugging
    public string? NfcRawData { get; set; }
}