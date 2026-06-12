namespace TemperaturMonitor.Services;

public record HardwareInfo(string Key, string Name, bool HasTemp, bool HasLoad, bool HasFan);

public interface IVisibilityService
{
    bool IsReady { get; }
    event Action? Changed;

    bool IsVisible(string key);
    void SetVisible(string key, bool visible);

    bool ShowsSensor(string entryKey, string sensor);
    void SetShowSensor(string entryKey, string sensor, bool show);

    void RegisterEntries(IEnumerable<HardwareInfo> entries);
    IReadOnlyList<HardwareInfo> KnownEntries { get; }
}