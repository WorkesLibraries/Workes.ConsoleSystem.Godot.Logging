# Architecture

## Boundary

A Godot C# logger adapter that forwards callbacks to a caller-owned ConsoleSystem manager. The package targets .NET 8, uses GodotSharp 4.7.2, and depends on ConsoleSystem 0.9.0. Godot.SourceGenerators 4.7.2 is private to the build.

## Flow

`GodotLogAttachment` registers an internal `Godot.Logger` with `OS.Singleton`. The logger translates message and error callbacks into `GodotLogKind`, derives message/source data, and calls an internal forwarder. The forwarder applies the default or supplied nullable mapping and records a `LogEntry` through `ConsoleManager.RecordEntry`.

Defaults are Information for standard output, Error for stderr and engine errors, Warning for engine warnings, and Critical for script/shader errors. Error messages prefer rationale, then error code; file, line, and function details become source metadata.

## Threading And Lifetime

Godot may call the logger concurrently on worker threads. Forwarding is serialized and does not call Godot back from the callback. Construct after runtime startup and dispose before shutdown. Disposal unregisters the logger and releases the manager reference, but does not dispose the manager. Godot supplies no .NET exception instance, and the adapter does not marshal UI work.
