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

    private sealed class GodotLogger : Logger
    {
        private readonly GodotLogForwarder _forwarder;
        internal GodotLogger(GodotLogForwarder forwarder) { _forwarder = forwarder; }

        public override void _LogMessage(string message, bool error) =>
            _forwarder.Write(error ? GodotLogKind.ErrorStream : GodotLogKind.Standard, message, "Godot");

        public override void _LogError(string function, string file, int line, string code, string rationale, bool editorNotify, int errorType, global::Godot.Collections.Array<global::Godot.ScriptBacktrace> scriptBacktraces)
        {
            GodotLogKind kind = errorType switch
            {
                1 => GodotLogKind.EngineWarning,
                2 => GodotLogKind.ScriptError,
                3 => GodotLogKind.ShaderError,
                _ => GodotLogKind.EngineError
            };
            string message = string.IsNullOrEmpty(rationale) ? code : rationale;
            string location = string.IsNullOrWhiteSpace(file) ? function : file + ":" + line + (string.IsNullOrWhiteSpace(function) ? string.Empty : " (" + function + ")");
            _forwarder.Write(kind, message, string.IsNullOrEmpty(location) ? "Godot" : location);
        }
    }
}


