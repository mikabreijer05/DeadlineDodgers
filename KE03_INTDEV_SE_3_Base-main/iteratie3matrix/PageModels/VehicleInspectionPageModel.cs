using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using iteratie3matrix.Models;

namespace iteratie3matrix.PageModels;

/// <summary>
/// WHAT:
/// Controls vehicle inspection workflow step.
///
/// WHY:
/// This is the gate between van selection and loading phase.
/// It ensures only safe vehicles continue.
/// </summary>
public partial class VehicleInspectionPageModel : ObservableObject
{
    private readonly DeliverySession _session;

    public VehicleInspectionPageModel(DeliverySession session)
    {
        _session = session;
    }

    // WHAT: Local UI state for damage toggle
    [ObservableProperty]
    private bool hasDamage;

    // WHAT: User reports damage found
    [RelayCommand]
    private async Task DamageFound()
    {
        HasDamage = true;

        _session.HasInspectionPassed = false;

        await Shell.Current.GoToAsync("//vehicledamage");
    }

    // WHAT: User confirms no damage
    [RelayCommand]
    private async Task NoDamage()
    {
        HasDamage = false;
        _session.HasInspectionPassed = true;

        await Shell.Current.DisplayAlert(
            "Voertuiginspectie",
            "Voertuig goedgekeurd voor gebruik.",
            "OK");
    }

    // WHAT: Starts loading phase
    // WHY: Moves workflow forward after inspection
    [RelayCommand]
    private async Task StartLoading()
    {
        if (_session.SelectedVan == null)
        {
            await Shell.Current.DisplayAlert(
                "Geen voertuig",
                "Selecteer eerst een voertuig.",
                "OK");
            return;
        }

        if (!_session.HasInspectionPassed)
        {
            await Shell.Current.DisplayAlert(
                "Geblokkeerd",
                "De voertuiginspectie is niet goedgekeurd.",
                "OK");
            return;
        }

        _session.IsLoading = true;

        await Shell.Current.GoToAsync("//cartscanner");
    }
}