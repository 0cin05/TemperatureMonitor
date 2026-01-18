namespace TemperaturMonitor.Services;

public class GraphService : IGraphService
{
    private readonly IClampService _clampService;

    public GraphService(IClampService clampService)
    {
        _clampService = clampService;
    }

    public void PushPoint(List<(DateTime t, float v)> series, float value)
    {
        var now = DateTime.Now;
        series.Add((now, value));

        var cutoff = now.AddMinutes(-1);
        int removeCount = 0;
        while (removeCount < series.Count && series[removeCount].t < cutoff)
            removeCount++;
        
        if(removeCount > 0)
            series.RemoveRange(0, removeCount);
    }
    
    public string BuildPolyline(List<(DateTime t, float v)> series, float minT, float maxT)
    {
        if (series.Count < 2) return "";

        int n = series.Count;
        var pts = new System.Text.StringBuilder();

        for (int i = 0; i < n; i++)
        {
            float x = (n == 1) ? 0 : (i * 100f / (n - 1));

            float v = _clampService.Clamp(series[i].v, minT, maxT);
            float y = 30f - ((v - minT) / (maxT - minT)) * 30f; // 0..30

            if (i > 0) pts.Append(' ');
            pts.Append(x.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture));
            pts.Append(',');
            pts.Append(y.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture));
        }

        return pts.ToString();
    }
}