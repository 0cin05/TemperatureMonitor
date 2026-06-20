using TemperatureMonitor.Models;

namespace TemperatureMonitor.Services;

public interface IHardwareService : IDisposable
{
    bool IsReady { get; }
    string? InitError { get; }
    IReadOnlyList<HardwareEntry> Entries { get; }
    event Action? Changed;
    void Reset();
}