# Workes.ConsoleSystem.Godot.Logging

An opt-in Godot custom logger that forwards the engine's supported log callbacks to a supplied Workes.ConsoleSystem `ConsoleManager`.

## Installation

```bash
dotnet add package Workes.ConsoleSystem.Godot.Logging --version 0.1.0
```

```xml
<PackageReference Include="Workes.ConsoleSystem.Godot.Logging" Version="0.1.0" />
```

## Usage

```csharp
using Workes.ConsoleSystem.Core;
using Workes.ConsoleSystem.Godot.Logging;

var manager = new ConsoleManager();
using var godotLogs = new GodotLogAttachment(manager);
```

The default mapping sends standard messages to Information, stderr messages and engine errors to Error, engine warnings to Warning, and script or shader errors to Critical. Supply a `Func<GodotLogKind, Workes.ConsoleSystem.Logging.LogLevel?>` to override the mapping; return `null` to ignore a source kind. Error records preserve the available file, line, function, code or rationale as message/source metadata. Godot's logger API does not provide a .NET `Exception` instance.

Targets .NET 8 and GodotSharp 4.7.2. Godot calls custom logger methods from arbitrary threads and may call them concurrently; this adapter synchronizes writes. Register it after the Godot runtime starts and dispose it before the runtime shuts down.

See [Quick Start](docs/QUICK_START.md) and [Changelog](CHANGELOG.md).

## License

MIT. See [LICENSE](LICENSE).
