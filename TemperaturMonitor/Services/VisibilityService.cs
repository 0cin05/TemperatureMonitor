using System.Text.Json;

namespace TemperaturMonitor.Services;

public class VisibilityService : IVisibilityService
{
    private static readonly string FilePath =
        Path.Combine(FileSystem.AppDataDirectory, "visibility.json");

    private static readonly string EntriesCachePath =
        Path.Combine(FileSystem.AppDataDirectory, "hardware_cache.json");

    private Dictionary<string, bool> _settings = new();
    private List<HardwareInfo> _knownEntries = new();

    public bool IsReady { get; private set; }
    public event Action? Changed;
    public IReadOnlyList<HardwareInfo> KnownEntries => _knownEntries;

    public VisibilityService()
    {
        Load();
        LoadCachedEntries();
    }

    public bool IsVisible(string key) =>
        !_settings.TryGetValue(key, out var v) || v;

    public void SetVisible(string key, bool visible)
    {
        _settings[key] = visible;
        Save();
        Changed?.Invoke();
    }

    public bool ShowsSensor(string entryKey, string sensor) =>
        !_settings.TryGetValue($"{entryKey}:{sensor}", out var v) || v;

    public void SetShowSensor(string entryKey, string sensor, bool show)
    {
        _settings[$"{entryKey}:{sensor}"] = show;
        Save();
        Changed?.Invoke();
    }

    public void RegisterEntries(IEnumerable<HardwareInfo> entries)
    {
        _knownEntries = entries.ToList();
        IsReady = true;
        SaveEntries();
        Changed?.Invoke();
    }

    private void LoadCachedEntries()
    {
        try
        {
            if (!File.Exists(EntriesCachePath)) return;
            var json = File.ReadAllText(EntriesCachePath);
            var cached = JsonSerializer.Deserialize<List<HardwareInfo>>(json);
            if (cached is { Count: > 0 })
            {
                _knownEntries = cached;
                IsReady = true;
            }
        }
        catch { }
    }

    private void Load()
    {
        try
        {
            if (File.Exists(FilePath))
            {
                var json = File.ReadAllText(FilePath);
                _settings = JsonSerializer.Deserialize<Dictionary<string, bool>>(json) ?? new();
            }
        }
        catch { _settings = new(); }
    }

    private void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
            File.WriteAllText(FilePath, JsonSerializer.Serialize(_settings));
        }
        catch { }
    }

    private void SaveEntries()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(EntriesCachePath)!);
            File.WriteAllText(EntriesCachePath, JsonSerializer.Serialize(_knownEntries));
        }
        catch { }
    }
}