using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using iteratie3matrix.DAL;
using iteratie3matrix.Models;
using Microsoft.Maui.Media;

namespace iteratie3matrix.PageModels;

public partial class OrderDetailPageModel : ObservableObject, IQueryAttributable
{
    private readonly OrderRepository _orderRepository;
    private readonly StatusRepository _statusRepository;
    private readonly AccountRepository _accountRepository;
    private readonly AddressRepository _addressRepository;

    // =========================
    // DELIVERY CORE DATA
    // =========================

    [ObservableProperty] private int orderId;
    [ObservableProperty] private DateTime orderDate;
    [ObservableProperty] private int statusId;
    [ObservableProperty] private string statusText = string.Empty;

    // =========================
    // HUMAN READABLE INFO
    // =========================

    [ObservableProperty] private string accountName = "";
    [ObservableProperty] private string customerName = "";
    [ObservableProperty] private string fullAddress = "";

    // =========================
    // PACKAGE INFO
    // =========================

    [ObservableProperty]
    private int totalPackages;

    // =========================
    // ORDER ITEMS (still needed for scanning future)
    // =========================

    [ObservableProperty]
    private List<OrderProductItem> orderItems = new();

    // =========================
    // DELIVERY PROOF
    // =========================

    [ObservableProperty]
    private string? deliveryPhotoPath;

    public OrderDetailPageModel(
        OrderRepository orderRepository,
        StatusRepository statusRepository,
        AccountRepository accountRepository,
        AddressRepository addressRepository)
    {
        _orderRepository = orderRepository;
        _statusRepository = statusRepository;
        _accountRepository = accountRepository;
        _addressRepository = addressRepository;
    }

    // =========================
    // LOAD DELIVERY
    // =========================

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.ContainsKey("id"))
            return;

        var id = Convert.ToInt32(query["id"]);

        var order = await _orderRepository.GetAsync(id);
        if (order is null)
            return;

        OrderId = order.OrderId;
        OrderDate = order.OrderDate;
        StatusId = order.StatusId;

        StatusText = await _statusRepository.GetNameAsync(order.StatusId);

        // =========================
        // ACCOUNT INFO
        // =========================
        var account = await _accountRepository.GetAsync(order.AccountId);
        if (account != null)
        {
            AccountName = account.AccountName;
            CustomerName = account.CustomerName;
        }

        // =========================
        // ADDRESS INFO
        // =========================
        var address = await _addressRepository.GetAsync(order.AddressId);
        if (address != null)
        {
            FullAddress =
                $"{address.Street} {address.HouseNumber}, " +
                $"{address.PostalCode} {address.City}, {address.Country}";
        }

        // =========================
        // PACKAGE COUNT (IMPORTANT)
        // =========================
        TotalPackages = await _orderRepository.GetTotalPackageCountAsync(order.OrderId);

        // ITEMS (for future scanning system)
        OrderItems = await _orderRepository.GetOrderProductsAsync(order.OrderId);
    }

    // =========================
    // START ROUTE
    // =========================

    [RelayCommand]
    private async Task StartRoute()
    {
        await _orderRepository.UpdateStatusAsync(OrderId, 4);

        StatusId = 4;
        StatusText = await _statusRepository.GetNameAsync(4);

        // Navigate to the Route Page
        await Shell.Current.GoToAsync("//route");
    }

    // =========================
    // DELIVERED
    // =========================

    [RelayCommand]
    private async Task MarkAsDelivered()
    {
        try
        {
            // =========================
            // CAMERA (DEMO PURPOSE ONLY)
            // =========================
            // WHAT:
            // Opens device camera and takes a photo.
            //
            // WHY:
            // Simulates proof-of-delivery for demonstration.
            // Stored image is not used for business logic.

            var photo = await MediaPicker.Default.CapturePhotoAsync();

            if (photo != null)
            {
                var filePath = Path.Combine(
                    FileSystem.CacheDirectory,
                    $"{OrderId}_delivery.jpg");

                await using var stream = await photo.OpenReadAsync();
                await using var file = File.OpenWrite(filePath);
                await stream.CopyToAsync(file);

                DeliveryPhotoPath = filePath;
            }

            // =========================
            // UPDATE STATUS
            // =========================
            await _orderRepository.UpdateStatusAsync(OrderId, 5);

            StatusId = 5;
            StatusText = await _statusRepository.GetNameAsync(5);

            // =========================
            // RETURN TO LIST
            // =========================
            await Shell.Current.GoToAsync("//orders");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert(
                "Fout",
                ex.Message,
                "OK");
        }
    }
}