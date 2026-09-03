using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Hosting;
using iteratie3matrix.DAL;
using iteratie3matrix.Models;
using iteratie3matrix.PageModels;
using iteratie3matrix.Pages;
using iteratie3matrix.Services;

namespace iteratie3matrix;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        // WHAT: Load application settings.
        // WHY: Makes configuration values available throughout the app.
        using var stream = FileSystem.OpenAppPackageFileAsync("appsettings.json")
            .GetAwaiter()
            .GetResult();

        var configuration = new ConfigurationBuilder()
            .AddJsonStream(stream)
            .Build();

        builder.Configuration.AddConfiguration(configuration);

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureSyncfusionToolkit();

#if DEBUG
        // WHAT: Enable debug logging.
        // WHY: Helps diagnose issues during development.
        builder.Logging.AddDebug();
#endif

        // =====================
        // DATA LAYER
        // =====================

        // WHAT: Register repositories and database access.
        // HOW: Dependency Injection creates shared instances where needed.
        builder.Services.AddSingleton<SQLDAL>();
        builder.Services.AddSingleton<OrderRepository>();
        builder.Services.AddSingleton<StatusRepository>();
        builder.Services.AddSingleton<DeliveryRepository>();
        builder.Services.AddSingleton<VehicleRepository>();
        builder.Services.AddSingleton<AccountRepository>();
        builder.Services.AddSingleton<AddressRepository>();

        // =====================
        // STATE
        // =====================

        // WHAT: Stores the current delivery session.
        // WHY: Keeps delivery data available between pages.
        builder.Services.AddSingleton<DeliverySession>();

        // =====================
        // PAGE MODELS
        // =====================

        // WHAT: Register ViewModels.
        // HOW: A new instance is created whenever a page is opened.
        builder.Services.AddTransient<OrderListPageModel>();
        builder.Services.AddTransient<OrderDetailPageModel>();
        builder.Services.AddTransient<VanSelectionPageModel>();
        builder.Services.AddTransient<VehicleInspectionPageModel>();
        builder.Services.AddTransient<LoginPageModel>();
        builder.Services.AddTransient<CartScannerPageModel>();
        builder.Services.AddTransient<VehicleDamagePageModel>();

        // =====================
        // PAGES
        // =====================

        // WHAT: Register application pages for Dependency Injection.
        builder.Services.AddTransient<OrderListPage>();
        builder.Services.AddTransient<OrderDetailPage>();
        builder.Services.AddTransient<VanSelectionPage>();
        builder.Services.AddTransient<VehicleInspectionPage>();
        builder.Services.AddTransient<CartScannerPage>();
        builder.Services.AddTransient<VehicleDamagePage>();

        // =====================
        // SERVICES
        // =====================

        // WHAT: Register the NFC communication service.
        // WHY: MainActivity and LoginPageModel must share the same instance.
        builder.Services.AddSingleton<NfcService>();

        // =====================
        // ROUTES
        // =====================

        // WHAT: Register Shell navigation routes.
        // WHY: Allows navigation using route names instead of page instances.
        builder.Services.AddTransientWithShellRoute<
            OrderDetailPage,
            OrderDetailPageModel>("order");

        return builder.Build();
    }
}