namespace TemperatureMonitor.Services;

public interface IColorService
{
    string LoadToColor(double load);
    string TempToColor(double t);
}