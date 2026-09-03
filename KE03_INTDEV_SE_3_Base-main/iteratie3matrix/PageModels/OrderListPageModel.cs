using CommunityToolkit.Mvvm.ComponentModel;
using iteratie3matrix.DAL;
using iteratie3matrix.Models;

namespace iteratie3matrix.PageModels;

public partial class OrderListPageModel : ObservableObject
{
    private readonly OrderRepository _orderRepository;
    private readonly StatusRepository _statusRepository;

    public OrderListPageModel(
        OrderRepository orderRepository,
        StatusRepository statusRepository)
    {
        _orderRepository = orderRepository;
        _statusRepository = statusRepository;
    }

    // =========================
    // ACTIVE DELIVERIES
    // =========================

    /*
        WHAT:
        Deliveries currently on the road.

        WHY:
        Allows the courier to continue
        deliveries after returning to the page.
    */
    [ObservableProperty]
    private List<OrderListItem> activeDeliveries = new();

    // =========================
    // AVAILABLE DELIVERIES
    // =========================

    /*
        WHAT:
        Deliveries ready for shipment.

        WHY:
        Couriers may only start these deliveries.
    */
    [ObservableProperty]
    private List<OrderListItem> deliveries = new();

    // =========================
    // UI STATE
    // =========================

    /*
        WHAT:
        Determines whether active deliveries exist.

        WHY:
        Prevents empty sections from appearing.
    */
    [ObservableProperty]
    private bool hasActiveDeliveries;

    // =========================
    // LOAD DELIVERIES
    // =========================

    /*
        WHAT:
        Loads all orders from the database.

        WHY:
        Splits orders into:
        - Active deliveries (Verzonden)
        - Available deliveries (Klaar om te verzenden)
    */
    public async Task LoadAsync()
    {
        var ordersFromDb =
            await _orderRepository.ListAsync();

        var statusMap =
            await _statusRepository.GetAllAsDictionaryAsync();

        // =========================
        // MAP DATABASE ORDERS
        // =========================

        var mapped =
            ordersFromDb
                .OrderBy(o => o.OrderDate)
                .Select(o => new OrderListItem
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    StatusId = o.StatusId,

                    StatusName =
                        statusMap.TryGetValue(
                            o.StatusId,
                            out var name)
                            ? name
                            : "Onbekend"
                })
                .ToList();

        // =========================
        // ACTIVE DELIVERIES
        // STATUS 4 = VERZONDEN
        // =========================

        ActiveDeliveries =
            mapped
                .Where(x => x.StatusId == 4)
                .OrderBy(x => x.OrderDate)
                .ToList();

        // =========================
        // AVAILABLE DELIVERIES
        // STATUS 3 = KLAAR OM TE VERZENDEN
        // =========================

        Deliveries =
            mapped
                .Where(x => x.StatusId == 3)
                .OrderBy(x => x.OrderDate)
                .ToList();

        // =========================
        // UPDATE UI
        // =========================

        /*
            WHAT:
            Controls visibility of the
            Active Deliveries section.

            WHY:
            The section should only appear
            when active routes exist.
        */
        HasActiveDeliveries =
            ActiveDeliveries.Count > 0;

#if DEBUG

        /*
            DEBUG:
            Remove later if desired.

            WHY:
            Confirms active deliveries
            are being loaded correctly.
        */
        System.Diagnostics.Debug.WriteLine(
            $"Active deliveries: {ActiveDeliveries.Count}");

#endif
    }
}