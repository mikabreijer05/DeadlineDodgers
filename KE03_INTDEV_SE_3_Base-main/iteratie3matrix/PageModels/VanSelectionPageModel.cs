using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using iteratie3matrix.Models;

namespace iteratie3matrix.PageModels;

/// <summary>
/// =====================
/// VAN SELECTION
/// =====================
/// WHAT:
/// Handles van selection for shift start
///
/// WHY:
/// Assigns vehicle from DB or session state
/// </summary>
public partial class VanSelectionPageModel : ObservableObject
{
    private readonly DeliverySession _session;

    public VanSelectionPageModel(DeliverySession session)
    {
        _session = session;
    }

    // =====================
    // VAN LIST
    // =====================
    [ObservableProperty]
    private List<Van> vans = new();

    // =====================
    // LOAD DATA
    // =====================
    public Task LoadAsync()
    {
        // TODO: replace with VehicleRepository (DB)
        Vans = new List<Van>
        {
            new() { VanId = 1, Name = "Van 12", LicensePlate = "V-123-KD", ParkingLocation = "MC-012" },
            new() { VanId = 2, Name = "Van 18", LicensePlate = "T-456-LP", ParkingLocation = "MC-018" },
            new() { VanId = 3, Name = "Van 24", LicensePlate = "R-789-ZX", ParkingLocation = "MC-024" }
        };

        return Task.CompletedTask;
    }

    // =====================
    // SHIFT START
    // =====================
    [RelayCommand]
    private async Task StartShift()
    {
        var van = Vans.FirstOrDefault();
        if (van == null)
            return;

        _session.SelectedVan = van;
        _session.ShiftStarted = true;

        await Shell.Current.DisplayAlert(
            "Dienst gestart",
            $"Voertuig: {van.Name}",
            "OK");

        await Shell.Current.GoToAsync("//vehicleinspection");
    }
}