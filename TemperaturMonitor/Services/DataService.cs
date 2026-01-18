using LibreHardwareMonitor.Hardware;

namespace TemperaturMonitor.Services;

public class DataService : IDataService
{
    public (float temp, float Load) GetCpuData(IHardware hw)
    {
        float temp = 0;
        float load = 0;
        
        foreach (var sensor in hw.Sensors)
        {
            if (sensor.SensorType == SensorType.Temperature &&
                sensor.Name == "Core Average" &&
                sensor.Value.HasValue)
            {
                temp = sensor.Value.Value;
            }

            if (sensor.SensorType == SensorType.Load &&
                sensor.Value.HasValue &&
                (sensor.Name.Contains("CPU Total", StringComparison.OrdinalIgnoreCase) ||
                 sensor.Name.Equals("Total", StringComparison.OrdinalIgnoreCase)))
            {
                load = sensor.Value.Value;
            }
        }
        
        return (temp, load);
    }

    //Gets the CPU-Fan RPM
    public float GetMainboardData(IHardware hw)
    {
        float rpm = 0;
        
        foreach (var sensor in hw.Sensors)
        {
            if (sensor.SensorType == SensorType.Fan && sensor.Value.HasValue)
            {
                if (sensor.Name.Equals("Fan #1", StringComparison.OrdinalIgnoreCase) ||
                    sensor.Name.Equals("Fan 1", StringComparison.OrdinalIgnoreCase) ||
                    sensor.Name.Contains("Fan #1", StringComparison.OrdinalIgnoreCase) ||
                    sensor.Name.Contains("Fan 1", StringComparison.OrdinalIgnoreCase))
                {
                    rpm = sensor.Value.Value;
                    break;
                }
            }
        }

        return rpm;
    }

    public (float temp, float Load, float RPM) GetGpuData(IHardware hw)
    {
        float temp = 0;
        float load = 0;
        float rpm = 0;
        
        foreach (var sensor in hw.Sensors)
        {
            if (sensor.SensorType == SensorType.Temperature && sensor.Value.HasValue)
            {
                temp = sensor.Value.Value;
                break;
            }
        }

        foreach (var sensor in hw.Sensors)
        {
            if (sensor.SensorType == SensorType.Load && sensor.Value.HasValue)
            {
                if (sensor.Name.Contains("GPU", StringComparison.OrdinalIgnoreCase) ||
                    sensor.Name.Contains("Core", StringComparison.OrdinalIgnoreCase) ||
                    sensor.Name.Contains("Total", StringComparison.OrdinalIgnoreCase))
                {
                    load = sensor.Value.Value;
                    break;
                }
            }
        }

        foreach (var sensor in hw.Sensors)
        {
            if (sensor.SensorType == SensorType.Fan && sensor.Value.HasValue)
            {
                rpm = sensor.Value.Value;
                break;
            }
        }
        
        return (temp, load, rpm);
    }

    public float GetRamData(IHardware hw)
    {
        float load = 0;
        
        foreach (var sensor in hw.Sensors)
        {
            if (sensor.SensorType == SensorType.Load && sensor.Value.HasValue)
            {
                load = sensor.Value.Value;
                break;
            }
        }
        return load;
    }

    private readonly Dictionary<string, int> _storageIndexById = new();
    private int _nextStorageIndex;
    float _ssd1Temp;
    float _ssd2Temp;
    public (float Ssd1Temp, float Ssd2Temp) GetStorageData(IHardware hw)
    {
        
        
        var id = hw.Identifier.ToString();

        if (!_storageIndexById.TryGetValue(id, out int idx))
        {
            idx = _nextStorageIndex++;
            _storageIndexById[id] = idx;
        }

        var tempSensor = hw.Sensors.FirstOrDefault(s =>
            s.SensorType == SensorType.Temperature &&
            s.Value.HasValue &&
            (s.Name.Equals("Temperature", StringComparison.OrdinalIgnoreCase) ||
             s.Name.Contains("Temperature", StringComparison.OrdinalIgnoreCase) ||
             s.Name.Contains("Drive", StringComparison.OrdinalIgnoreCase)));

        if (tempSensor?.Value is { } temp)
        {
            if (idx == 0)
            {
                _ssd1Temp = temp;
            }
            else if (idx == 1)
            {
                _ssd2Temp = temp;
            }
        }
        
        return  (_ssd1Temp, _ssd2Temp);
    }
}