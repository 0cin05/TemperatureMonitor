# TemperaturMonitor

> **Sprache:** [English](README.md) | Deutsch

Eine Windows-Desktop-App zur Echtzeit-Überwachung von Hardware-Sensoren — Temperaturen, Auslastung, Lüfterdrehzahl und RAM-Verbrauch — mit Live-Verlaufsgraphen für jede Komponente.

Entwickelt mit **.NET 9 / MAUI Blazor Hybrid** und **OpenHardwareMonitor**.

---

## Funktionen

- CPU-, GPU- (Nvidia & AMD), RAM- und Speicher-Temperatur- und Auslastungsüberwachung
- Lüfterdrehzahl (CPU- & GPU-Lüfter)
- Maximale Temperatur pro Komponente
- Scrollender Live-Verlaufsgraph pro Komponente
- Benötigt Administratorrechte für den Hardware-Zugriff

## Systemvoraussetzungen

- Windows 10 Version 1803 (Build 17763) oder neuer
- x64-Prozessor
- Administratorrechte (werden von OpenHardwareMonitor für den Zugriff auf Hardware-Sensoren benötigt)

---

## Installation

### Option 1 — Installer (empfohlen)

1. Zur [Releases](https://github.com/0cin05/TemperaturMonitor/releases)-Seite wechseln.
2. Die neueste Datei `TemperaturMonitor-vX.X.X-setup.exe` herunterladen.
3. Den Installer ausführen. Er fragt nach Administrator-Berechtigung.
4. **TemperaturMonitor** über die Desktop-Verknüpfung oder das Startmenü starten.

### Option 2 — Aus dem Quellcode bauen

**Voraussetzungen**

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- .NET MAUI Workload für Windows

```bash
# MAUI Workload installieren (einmalig)
dotnet workload install maui-windows

# Repository klonen
git clone https://github.com/0cin05/TemperaturMonitor.git
cd TemperaturMonitor

# Self-contained Build veröffentlichen
dotnet publish TemperaturMonitor/TemperaturMonitor.csproj \
  -f net9.0-windows10.0.19041.0 \
  -c Release \
  -r win-x64 \
  --self-contained true \
  -o ./publish
```

Die Ausgabe liegt im Ordner `publish/`. `TemperaturMonitor.exe` **als Administrator** starten.

> **Optional — Installer bauen**
> Benötigt [Inno Setup](https://jrsoftware.org/isinfo.php).
>
> ```bash
> iscc installer/setup.iss /DAppVersion=v1.0.0
> # Ausgabe: installer/Output/TemperaturMonitor-v1.0.0-setup.exe
> ```

---

## Lizenz

MIT — siehe [LICENSE](LICENSE).
