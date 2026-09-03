using iteratie3matrix.DAL;

namespace iteratie3matrix;

public partial class App : Application
{
    private readonly OrderRepository _orderRepository;

    public App(OrderRepository orderRepository)
    {
        InitializeComponent();
        _orderRepository = orderRepository;

        MainPage = new AppShell();
    }

    protected override async void OnStart()
    {
        base.OnStart();

        // =========================
        // DATABASE WARM-UP
        // =========================
        try
        {
            await _orderRepository.ListAsync();
        }
        catch
        {
            // ignore warm-up failure (app should still run)
        }
    }
}