using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using iteratie3matrix.DAL;
using iteratie3matrix.Models;

namespace iteratie3matrix.PageModels;

public partial class CartScannerPageModel : ObservableObject
{
    private readonly DeliverySession _session;
    private readonly DeliveryRepository _repo;

    public CartScannerPageModel(
        DeliverySession session,
        DeliveryRepository repo)
    {
        _session = session;
        _repo = repo;
    }

    // =====================
    // CART DATA
    // =====================

    [ObservableProperty]
    private CartLoad? assignedCart;

    // =====================
    // SCAN STATE
    // =====================

    [ObservableProperty]
    private string lastScanResult = string.Empty;

    // =====================
    // LOAD CART
    // =====================

    public async Task LoadAsync()
    {
        if (_session.SelectedVan == null)
            return;

        AssignedCart =
            await _repo.GetCartForVehicleAsync(
                _session.SelectedVan.VanId);
    }

    // =====================
    // SCAN PACKAGE
    // =====================

    [RelayCommand]
    private void ScanCart()
    {
        if (AssignedCart == null)
            return;

        var scannedBarcode = "1";

        var item =
            AssignedCart.Items
                .FirstOrDefault(x =>
                    x.ProductId.ToString() == scannedBarcode &&
                    x.Remaining > 0);

        if (item == null)
        {
            LastScanResult = "Ongeldige scan";
            return;
        }

        item.Remaining--;
        AssignedCart.ScannedPackages++;

        LastScanResult =
            $"Product {item.ProductId} gescand";

        OnPropertyChanged(nameof(AssignedCart));
    }

    // =====================
    // CONTINUE FLOW
    // =====================

    [RelayCommand]
    private async Task ContinueToOrders()
    {
        if (AssignedCart == null)
            return;

        if (AssignedCart.ScannedPackages <
            AssignedCart.ExpectedPackages)
        {
            await Shell.Current.DisplayAlert(
                "Niet voltooid",
                "Nog niet alle pakketten zijn gescand.",
                "OK");

            return;
        }

        _session.IsCartComplete = true;

        await Shell.Current.GoToAsync("//orders");
    }

    // =====================
    // SKIP FLOW (OPTIONAL) demonstration only
    // =====================

    [RelayCommand]
    private async Task SkipScanning()
    {
        if (AssignedCart == null)
            return;

        _session.IsCartComplete = true;

        AssignedCart.ScannedPackages =
            AssignedCart.ExpectedPackages;

        await Shell.Current.GoToAsync("//orders");
    }
}