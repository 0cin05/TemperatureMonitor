namespace TemperatureMonitor.Services;

public interface IGraphService
{
    void PushPoint(List<(DateTime t, float v)> series, float value);
    string BuildPolyline(List<(DateTime t, float v)> series, float minT, float maxT);
}