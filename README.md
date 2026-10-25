# Workes.ConsoleSystem.Godot.Logging

An opt-in Godot C# custom logger that forwards Godot's supported log callbacks to a supplied Workes.ConsoleSystem `ConsoleManager`, while Godot continues to display its normal output.

## Installation

Install the package from [NuGet](https://www.nuget.org/packages/Workes.ConsoleSystem.Godot.Logging):

```bash
dotnet add package Workes.ConsoleSystem.Godot.Logging --version 1.0.0
```

```xml
<PackageReference Include="Workes.ConsoleSystem.Godot.Logging" Version="1.0.0" />
```

## Usage

```csharp
using Workes.ConsoleSystem.Core;
using Workes.ConsoleSystem.Godot.Logging;

var manager = new ConsoleManager();
var godotLogs = new GodotLogAttachment(manager);
// Keep godotLogs for the desired logging lifetime, then call Dispose().
```

The default mapping sends standard messages to Information, stderr messages and engine errors to Error, engine warnings to Warning, and script or shader errors to Critical. Supply a `Func<GodotLogKind, Workes.ConsoleSystem.Logging.LogLevel?>` to override the mapping; return `null` to ignore a source kind. For engine errors, the message uses the rationale when provided and otherwise the error code; available file, line, and function details are stored as the source. Godot's logger API does not provide a .NET `Exception` instance.

Targets .NET 8 and is built against GodotSharp 4.7.2. The consumer integration was tested with the Godot 4.7.2 .NET edition. Godot may invoke logger callbacks concurrently from worker threads; this adapter synchronizes writes. Register it after the Godot runtime starts and dispose it before the runtime shuts down. It cannot capture messages emitted before registration.

See [Quick Start](docs/QUICK_START.md) and [Changelog](CHANGELOG.md).

## License

MIT. See [LICENSE](LICENSE).
