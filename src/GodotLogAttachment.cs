using System;
using Godot;
using Workes.ConsoleSystem.Core;
using ConsoleLogLevel = Workes.ConsoleSystem.Logging.LogLevel;

namespace Workes.ConsoleSystem.Godot.Logging;

/// <summary>Registers a custom Godot logger and forwards messages to a supplied ConsoleManager.</summary>
/// <remarks>Godot invokes logger callbacks on arbitrary threads and may invoke them concurrently. This adapter synchronizes forwarding and does not call back into Godot from a logger callback.</remarks>
public sealed class GodotLogAttachment : IDisposable
{
    private readonly GodotLogger _logger;
    private readonly GodotLogForwarder _forwarder;
    private int _disposed;

    /// <summary>Creates and registers an adapter with the Godot OS singleton.</summary>
    public GodotLogAttachment(ConsoleManager manager, Func<GodotLogKind, ConsoleLogLevel?>? levelMapper = null)
    {
        if (manager is null) throw new ArgumentNullException(nameof(manager));
        _forwarder = new GodotLogForwarder(manager, levelMapper ?? GodotLogMapper.DefaultMap);
        _logger = new GodotLogger(_forwarder);
        OS.Singleton.AddLogger(_logger);
    }

    /// <summary>Unregisters the logger and releases the manager reference.</summary>
    public void Dispose()
    {
        if (System.Threading.Interlocked.Exchange(ref _disposed, 1) != 0) return;
        OS.Singleton.RemoveLogger(_logger);
        _forwarder.Dispose();
    }

}


