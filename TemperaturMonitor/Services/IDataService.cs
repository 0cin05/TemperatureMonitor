using LibreHardwareMonitor.Hardware;

namespace TemperaturMonitor.Services;

public interface IDataService
{
    (float temp, float Load) GetCpuData(IHardware hw);
    float GetMainboardData(IHardware hw);
    (float temp, float Load, float RPM) GetGpuData(IHardware hw);
    float GetRamData(IHardware hw);
    (float Ssd1Temp, float Ssd2Temp) GetStorageData(IHardware hw);
}