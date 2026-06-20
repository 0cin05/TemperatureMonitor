# TemperatureMonitor

> **Language:** English | [Deutsch](README.de.md)

A Windows desktop app that monitors hardware sensors in real time — temperatures, load, fan RPM, and RAM usage — with a live history graph for each component.

Built with **.NET 9 / MAUI Blazor Hybrid** and powered by **OpenHardwareMonitor**.

<img width="1917" height="1029" alt="Screenshot 2026-06-20 212026" src="https://github.com/user-attachments/assets/546a310d-87a0-4b87-8b6a-0495044abe3b" />
<img width="1917" height="1027" alt="Screenshot 2026-06-20 212033" src="https://github.com/user-attachments/assets/44f803ad-e724-4fe6-89b4-abad333dde6d" />

---

## Features

- CPU, GPU (Nvidia & AMD), RAM, and storage temperature & load monitoring
- Fan RPM readout (CPU & GPU fans)
- Max temperature tracking per component
- Live scrolling history graph per component
- Requires administrator privileges for hardware access

## Requirements

- Windows 10 version 1803 (build 17763) or later
- x64 processor
- Administrator rights (needed by OpenHardwareMonitor to access hardware sensors)

---

## Installation

### Option 1 — Installer (recommended)

1. Go to the [Releases](https://github.com/0cin05/TemperatureMonitor/releases) page.
2. Download the latest `TemperatureMonitor-vX.X.X-setup.exe`.
3. Run the installer. It will ask for administrator permission.
4. Launch **TemperatureMonitor** from the desktop shortcut or the Start menu.

### Option 2 — Build from source

**Prerequisites**

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- .NET MAUI workload for Windows

```bash
# Install the MAUI workload (one-time setup)
dotnet workload install maui-windows

# Clone the repository
git clone https://github.com/0cin05/TemperatureMonitor.git
cd TemperatureMonitor

# Publish a self-contained build
dotnet publish TemperatureMonitor/TemperatureMonitor.csproj \
  -f net9.0-windows10.0.19041.0 \
  -c Release \
  -r win-x64 \
  --self-contained true \
  -o ./publish
```

The output is in the `publish/` folder. Run `TemperatureMonitor.exe` **as administrator**.

> **Optional — build the installer**
> Requires [Inno Setup](https://jrsoftware.org/isinfo.php).
>
> ```bash
> iscc installer/setup.iss /DAppVersion=v1.0.0
> # Output: installer/Output/TemperatureMonitor-v1.0.0-setup.exe
> ```

---

## License

MIT — see [LICENSE](LICENSE).
