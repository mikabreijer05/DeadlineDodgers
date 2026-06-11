using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Hosting;

namespace iteratie3matrix;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureSyncfusionToolkit();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // =====================
        // REPOSITORIES
        // =====================
        builder.Services.AddSingleton<OrderRepository>();

        // =====================
        // PAGES + MODELS
        // =====================
        builder.Services.AddTransient<OrderListPageModel>();
        builder.Services.AddTransient<OrderListPage>();

        builder.Services.AddTransient<OrderDetailPageModel>();
        builder.Services.AddTransient<OrderDetailPage>();
        builder.Services.AddTransientWithShellRoute<OrderDetailPage, OrderDetailPageModel>("order");
        return builder.Build();
    }
}