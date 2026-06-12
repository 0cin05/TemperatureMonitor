using TemperaturMonitor.WinUI;

namespace TemperaturMonitor;

public partial class App : Application
{
    public App()
    {
        // WebView2 needs a user-writable data folder when installed under Program Files
        Environment.SetEnvironmentVariable("WEBVIEW2_USER_DATA_FOLDER",
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                         "TemperaturMonitor", "WebView2"));

        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(new MainPage()) { Title = "TemperaturMonitor" };

        window.Created += (_, __) =>
        {
            WindowsWindowPlacement.Restore(window);
            WindowsWindowPlacement.HookAndPersist(window);
        };

        return window;
    }
}