using OpenHardwareMonitor.Hardware;
using TemperaturMonitor.Models;

namespace TemperaturMonitor.Services;

public class HardwareService : IHardwareService
{
    private readonly IVisibilityService _visibility;
    private readonly IGraphService _graph;
    private Computer? _computer;
    private readonly List<HardwareEntry> _entries = new();
    private readonly CancellationTokenSource _cts = new();
    private bool _initCompleted;

    public bool IsReady { get; private set; }
    public string? InitError { get; private set; }
    public IReadOnlyList<HardwareEntry> Entries => _entries;
    public event Action? Changed;

    public HardwareService(IVisibilityService visibility, IGraphService graph)
    {
        _visibility = visibility;
        _graph = graph;
        _ = InitAsync();
    }

    private async Task InitAsync()
    {
        try
        {
            _computer = new Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true,
                IsMemoryEnabled = true,
                IsMotherboardEnabled = true,
                IsStorageEnabled = true,
            };

            await Task.Run(() =>
            {
                _computer.Open(false);
                DiscoverHardware();
                _initCompleted = true;
            });

            _visibility.RegisterEntries(
                _entries.Select(e => new HardwareInfo(e.Key, e.Name, e.HasTemp, e.HasLoad, e.HasFan))
            );

            IsReady = true;
            Changed?.Invoke();
            _ = PollAsync(_cts.Token);
        }
        catch (Exception ex)
        {
            InitError = ex.ToString();
            Changed?.Invoke();
        }
    }

    private async Task PollAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            foreach (var entry in _entries)
                UpdateEntry(entry);
            Changed?.Invoke();
            try { await Task.Delay(1000, token); }
            catch (OperationCanceledException) { break; }
        }
    }

    private static int SortOrder(HardwareEntry e) => e.Type switch
    {
        HardwareType.Cpu       => 0,
        HardwareType.GpuNvidia => 1,
        HardwareType.GpuAmd    => 1,
        HardwareType.Storage   => 2,
        HardwareType.Memory    => 3,
        _                      => 99,
    };

    private void DiscoverHardware()
    {
        IHardware? superIo = null;
        foreach (var hw in _computer!.Hardware)
        {
            if (hw.HardwareType != HardwareType.Motherboard) continue;
            foreach (var sub in hw.SubHardware)
            {
                sub.Update();
                if (sub.HardwareType == HardwareType.SuperIO &&
                    sub.Sensors.Any(s => s.SensorType == SensorType.Fan))
                {
                    superIo = sub;
                    break;
                }
            }
        }

        foreach (var hw in _computer!.Hardware)
        {
            hw.Update();

            switch (hw.HardwareType)
            {
                case HardwareType.Cpu:
                    _entries.Add(new HardwareEntry
                    {
                        Name        = hw.Name,
                        Type        = hw.HardwareType,
                        Hardware    = hw,
                        HasTemp     = hw.Sensors.Any(s => s.SensorType == SensorType.Temperature),
                        HasLoad     = hw.Sensors.Any(s => s.SensorType == SensorType.Load),
                        HasFan      = superIo != null,
                        FanHardware = superIo,
                    });
                    break;

                case HardwareType.GpuNvidia:
                case HardwareType.GpuAmd:
                    _entries.Add(new HardwareEntry
                    {
                        Name     = hw.Name,
                        Type     = hw.HardwareType,
                        Hardware = hw,
                        HasTemp  = hw.Sensors.Any(s => s.SensorType == SensorType.Temperature),
                        HasLoad  = hw.Sensors.Any(s => s.SensorType == SensorType.Load),
                        HasFan   = hw.Sensors.Any(s => s.SensorType == SensorType.Fan),
                    });
                    break;

                case HardwareType.Memory:
                    if (hw.Name.Contains("Virtual", StringComparison.OrdinalIgnoreCase))
                        break;
                    if (hw.Sensors.Any(s => s.SensorType == SensorType.Load))
                    {
                        _entries.Add(new HardwareEntry
                        {
                            Name     = "RAM",
                            Type     = hw.HardwareType,
                            Hardware = hw,
                            HasLoad  = true,
                        });
                    }
                    break;

                case HardwareType.Storage:
                    bool hasTemp = hw.Sensors.Any(s => s.SensorType == SensorType.Temperature);
                    bool hasLoad = hw.Sensors.Any(s => s.SensorType == SensorType.Load);
                    bool hasFan  = hw.Sensors.Any(s => s.SensorType == SensorType.Fan);
                    if (hasTemp || hasLoad || hasFan)
                    {
                        _entries.Add(new HardwareEntry
                        {
                            Name     = hw.Name,
                            Type     = hw.HardwareType,
                            Hardware = hw,
                            HasTemp  = hasTemp,
                            HasLoad  = hasLoad,
                            HasFan   = hasFan,
                        });
                    }
                    break;
            }
        }

        _entries.Sort((a, b) => SortOrder(a).CompareTo(SortOrder(b)));
    }

    private void UpdateEntry(HardwareEntry entry)
    {
        entry.Hardware.Update();
        entry.FanHardware?.Update();

        if (entry.HasTemp)
        {
            ISensor? sensor = null;

            if (entry.Type == HardwareType.Cpu)
            {
                sensor = entry.Hardware.Sensors.FirstOrDefault(s =>
                    s.SensorType == SensorType.Temperature &&
                    s.Value.HasValue &&
                    s.Name.Equals("Core Average", StringComparison.OrdinalIgnoreCase));
            }

            sensor ??= entry.Hardware.Sensors.FirstOrDefault(s =>
                s.SensorType == SensorType.Temperature && s.Value.HasValue);

            entry.Temp = sensor?.Value ?? 0f;
            if (entry.Temp > entry.MaxTemp) entry.MaxTemp = entry.Temp;
            _graph.PushPoint(entry.Points, entry.Temp);
        }

        if (entry.HasLoad)
        {
            ISensor? sensor = entry.Type switch
            {
                HardwareType.Cpu => entry.Hardware.Sensors.FirstOrDefault(s =>
                    s.SensorType == SensorType.Load && s.Value.HasValue &&
                    (s.Name.Contains("CPU Total", StringComparison.OrdinalIgnoreCase) ||
                     s.Name.Equals("Total", StringComparison.OrdinalIgnoreCase))),
                HardwareType.GpuNvidia or HardwareType.GpuAmd => entry.Hardware.Sensors.FirstOrDefault(s =>
                    s.SensorType == SensorType.Load && s.Value.HasValue &&
                    (s.Name.Contains("GPU", StringComparison.OrdinalIgnoreCase) ||
                     s.Name.Contains("Core", StringComparison.OrdinalIgnoreCase) ||
                     s.Name.Contains("Total", StringComparison.OrdinalIgnoreCase))),
                _ => entry.Hardware.Sensors.FirstOrDefault(s =>
                    s.SensorType == SensorType.Load && s.Value.HasValue),
            };

            entry.Load = sensor?.Value ?? 0f;
        }

        if (entry.Type == HardwareType.Memory)
        {
            var used = entry.Hardware.Sensors.FirstOrDefault(s =>
                s.SensorType == SensorType.Data && s.Value.HasValue &&
                s.Name.Contains("Used", StringComparison.OrdinalIgnoreCase));
            var available = entry.Hardware.Sensors.FirstOrDefault(s =>
                s.SensorType == SensorType.Data && s.Value.HasValue &&
                s.Name.Contains("Available", StringComparison.OrdinalIgnoreCase));

            entry.RamUsedGb  = used?.Value ?? 0f;
            entry.RamTotalGb = (used?.Value ?? 0f) + (available?.Value ?? 0f);
        }

        if (entry.HasFan)
        {
            var fanHw = entry.FanHardware ?? entry.Hardware;
            ISensor? sensor = null;

            if (entry.FanHardware != null)
            {
                sensor = fanHw.Sensors.FirstOrDefault(s =>
                    s.SensorType == SensorType.Fan && s.Value.HasValue &&
                    (s.Name.Contains("Fan #1", StringComparison.OrdinalIgnoreCase) ||
                     s.Name.Contains("Fan 1", StringComparison.OrdinalIgnoreCase)));
            }

            sensor ??= fanHw.Sensors.FirstOrDefault(s =>
                s.SensorType == SensorType.Fan && s.Value.HasValue);

            entry.FanRpm = sensor?.Value ?? 0f;
        }
    }

    public void Reset()
    {
        foreach (var entry in _entries)
        {
            entry.MaxTemp = 0;
            entry.Points.Clear();
        }
    }

    public void Dispose()
    {
        try { _cts.Cancel(); } catch { }
        _cts.Dispose();
        if (_initCompleted)
            try { _computer?.Close(); } catch { }
    }
}