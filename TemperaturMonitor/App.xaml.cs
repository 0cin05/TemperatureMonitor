using TemperaturMonitor.WinUI;

namespace TemperaturMonitor;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(new MainPage()) { Title = "TemperaturMonitor" };
        
#if WINDOWS
        window.Created += (_, __) =>
        {
            WindowsWindowPlacement.Restore(window);
            WindowsWindowPlacement.HookAndPersist(window);
        };
#endif

        return window;
    }
}