using TemperaturMonitor.Models;

namespace TemperaturMonitor.Services;

public interface IHardwareService : IDisposable
{
    bool IsReady { get; }
    string? InitError { get; }
    IReadOnlyList<HardwareEntry> Entries { get; }
    event Action? Changed;
    void Reset();
}