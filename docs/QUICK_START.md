# Quick Start

In a Godot 4.7.2 C# project, create the manager and attach the logger after the engine has started. Godot's existing output remains visible; matching entries are also added to the manager's history.

```csharp
#nullable enable

using System.Linq;
using Godot;
using Workes.ConsoleSystem.Core;
using Workes.ConsoleSystem.Entries;
using Workes.ConsoleSystem.Godot.Logging;

public partial class LoggingSetup : Node
{
    private ConsoleManager? _manager;
    private GodotLogAttachment? _godotLogs;

    public override void _Ready()
    {
        _manager = new ConsoleManager();
        _godotLogs = new GodotLogAttachment(_manager);

        GD.Print("Captured by Godot and ConsoleSystem");
        LogEntry[] entries = _manager.History.Entries.OfType<LogEntry>().ToArray();
        // Read entries here or forward them to your own UI or storage.
    }

    public override void _ExitTree()
    {
        _godotLogs?.Dispose();
        _godotLogs = null;
    }
}
```

The default mapping is Information for standard messages, Error for stderr and engine errors, Warning for engine warnings, and Critical for script or shader errors. Pass a `Func<GodotLogKind, Workes.ConsoleSystem.Logging.LogLevel?>` as the second constructor argument to customize it; return `null` to ignore a kind.

Godot can call logger callbacks concurrently on worker threads, so avoid calling Godot logging methods from code that processes captured entries. Register after the Godot runtime starts; messages emitted before registration are not captured. Dispose the attachment before the runtime shuts down.
