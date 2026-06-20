namespace TemperatureMonitor.Services;

public interface IClampService
{
    double Clamp(double v, double min, double max);
    float Clamp(float v, float min, float max);
}