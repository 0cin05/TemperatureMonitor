using OpenHardwareMonitor.Hardware;

namespace TemperaturMonitor.Models;

public class HardwareEntry
{
    public string Name { get; set; } = "";
    public HardwareType Type { get; set; }
    
    public bool HasTemp { get; set; }
    public bool HasLoad { get; set; }
    public bool HasFan  { get; set; }
    
    public float Temp   { get; set; }
    public float Load   { get; set; }
    public float FanRpm { get; set; }
    
    public double MaxTemp { get; set; }
    public List<(DateTime t, float v)> Points { get; set; } = new();
    
    public IHardware Hardware { get; set; } = null!;
    public IHardware? FanHardware { get; set; }

    public string Key => $"{Type}:{Name}";
}