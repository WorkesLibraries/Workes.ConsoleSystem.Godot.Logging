using Godot;

namespace Workes.ConsoleSystem.Godot.Logging;

/// <summary>Godot logger implementation registered by <see cref="GodotLogAttachment"/>.</summary>
internal sealed partial class GodotLogger : Logger
{
    private readonly GodotLogForwarder _forwarder;

    internal GodotLogger(GodotLogForwarder forwarder) =>
        _forwarder = forwarder ?? throw new System.ArgumentNullException(nameof(forwarder));

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
