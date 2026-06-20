using Microsoft.Extensions.Logging;
using TemperatureMonitor.Services;

namespace TemperatureMonitor;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddSingleton<IColorService, ColorService>();
        builder.Services.AddSingleton<IClampService, ClampService>();
        builder.Services.AddSingleton<IGraphService, GraphService>();
        builder.Services.AddSingleton<IVisibilityService, VisibilityService>();
        builder.Services.AddSingleton<IHardwareService, HardwareService>();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();

        // resolve now to start hardware init before the first page renders
        app.Services.GetRequiredService<IHardwareService>();

        return app;
    }
}