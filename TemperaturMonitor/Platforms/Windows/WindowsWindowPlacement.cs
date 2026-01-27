#if WINDOWS
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using Microsoft.UI.Windowing;
using Windows.Graphics;
using WinRT.Interop;

namespace TemperaturMonitor.WinUI;

static class WindowsWindowPlacement
{
    const string KX = "win_x";
    const string KY = "win_y";
    const string KW = "win_w";
    const string KH = "win_h";
    const string KState = "win_state"; // 0=Restored, 1=Maximized, 2=Minimized

    static AppWindow? GetAppWindow(Window mauiWindow)
    {
        var nativeWindow = (Microsoft.UI.Xaml.Window?)mauiWindow.Handler?.PlatformView;
        if (nativeWindow is null) return null;

        var hwnd = WindowNative.GetWindowHandle(nativeWindow);
        var id = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
        return AppWindow.GetFromWindowId(id);
    }

    public static void HookAndPersist(Window mauiWindow)
    {
        var appWindow = GetAppWindow(mauiWindow);
        if (appWindow is null) return;

        appWindow.Changed += (s, args) =>
        {
            if (args.DidPositionChange || args.DidSizeChange || args.DidPresenterChange)
                SaveNow(s);
        };

        mauiWindow.Stopped += (_, __) => SaveNow(appWindow);
    }

    static void SaveNow(AppWindow appWindow)
    {
        Preferences.Set(KX, appWindow.Position.X);
        Preferences.Set(KY, appWindow.Position.Y);
        Preferences.Set(KW, appWindow.Size.Width);
        Preferences.Set(KH, appWindow.Size.Height);

        int state = 0;
        if (appWindow.Presenter is OverlappedPresenter op)
        {
            state = op.State switch
            {
                OverlappedPresenterState.Maximized => 1,
                OverlappedPresenterState.Minimized => 2,
                _ => 0
            };
        }
        Preferences.Set(KState, state);
    }

    public static void Restore(Window mauiWindow)
    {
        var appWindow = GetAppWindow(mauiWindow);
        if (appWindow is null) return;

        int x = Preferences.Get(KX, int.MinValue);
        int y = Preferences.Get(KY, int.MinValue);
        int w = Preferences.Get(KW, -1);
        int h = Preferences.Get(KH, -1);
        int state = Preferences.Get(KState, 0);

        // noch nie gespeichert? -> nix tun
        if (x == int.MinValue || y == int.MinValue || w <= 0 || h <= 0)
            return;

        // Monitor-Fallback: wenn sich Monitore geändert haben
        var displayArea = DisplayArea.GetFromPoint(new PointInt32(x, y), DisplayAreaFallback.Nearest);
        if (displayArea != null)
        {
            var work = displayArea.WorkArea;
            int cx = Math.Clamp(x, work.X, work.X + work.Width - 50);
            int cy = Math.Clamp(y, work.Y, work.Y + work.Height - 50);
            appWindow.MoveAndResize(new RectInt32(cx, cy, w, h));
        }
        else
        {
            appWindow.MoveAndResize(new RectInt32(x, y, w, h));
        }

        if (appWindow.Presenter is OverlappedPresenter op)
        {
            if (state == 1) op.Maximize();
            else if (state == 2) op.Minimize();
            else op.Restore();
        }
    }
}
#endif
