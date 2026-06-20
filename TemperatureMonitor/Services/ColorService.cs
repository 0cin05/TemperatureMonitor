namespace TemperatureMonitor.Services;

public class ColorService : IColorService
{
    public string LoadToColor(double load)
    {
        if (load < 50) return "#22c55e";
        if (load < 85) return "#eab308";
        return "#ef4444";
    }

    public string TempToColor(double t)
    {
        if (t < 55) return "#22c55e";
        if (t < 75) return "#eab308";
        return "#ef4444";
    }
}