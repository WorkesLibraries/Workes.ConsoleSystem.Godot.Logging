using System;
using Workes.ConsoleSystem.Core;
using Workes.ConsoleSystem.Entries;
using ConsoleLogLevel = Workes.ConsoleSystem.Logging.LogLevel;

namespace Workes.ConsoleSystem.Godot.Logging;

/// <summary>Identifies the source channel of a Godot log callback.</summary>
public enum GodotLogKind
{
    /// <summary>A standard message written to stdout.</summary>
    Standard,
    /// <summary>A message written to stderr.</summary>
    ErrorStream,
    /// <summary>An engine error.</summary>
    EngineError,
    /// <summary>An engine warning.</summary>
    EngineWarning,
    /// <summary>A script error.</summary>
    ScriptError,
    /// <summary>A shader error.</summary>
    ShaderError
}

/// <summary>Maps Godot callback kinds to ConsoleSystem levels.</summary>
public static class GodotLogMapper
{
    /// <summary>Maps a callback kind to the default level; returns null for unrecognized kinds.</summary>
    public static ConsoleLogLevel? DefaultMap(GodotLogKind kind) => kind switch
    {
        GodotLogKind.Standard => ConsoleLogLevel.Information,
        GodotLogKind.ErrorStream or GodotLogKind.EngineError => ConsoleLogLevel.Error,
        GodotLogKind.EngineWarning => ConsoleLogLevel.Warning,
        GodotLogKind.ScriptError or GodotLogKind.ShaderError => ConsoleLogLevel.Critical,
        _ => null
    };
}

internal sealed class GodotLogForwarder : IDisposable
{
    private readonly object _gate = new object();
    private readonly Func<GodotLogKind, ConsoleLogLevel?> _mapper;
    private ConsoleManager? _manager;
    internal GodotLogForwarder(ConsoleManager manager, Func<GodotLogKind, ConsoleLogLevel?> mapper)
    { _manager = manager ?? throw new ArgumentNullException(nameof(manager)); _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper)); }
    internal void Write(GodotLogKind kind, string message, string? source)
    {
        lock (_gate)
        {
            if (_manager is null) return;
            ConsoleLogLevel? level = _mapper(kind);
            if (level.HasValue) _manager.RecordEntry(new LogEntry(DateTimeOffset.UtcNow, level.Value, message, source));
        }
    }
    public void Dispose() { lock (_gate) _manager = null; }
}

