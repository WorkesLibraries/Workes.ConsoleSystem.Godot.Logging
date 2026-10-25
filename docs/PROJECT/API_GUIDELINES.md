# API Guidelines

Construct `GodotLogAttachment(manager, levelMapper)` after Godot starts; construction registers the custom logger. Keep the attachment and dispose it before runtime shutdown. The caller retains ownership of the manager.

The optional `Func<GodotLogKind, LogLevel?>` selects the ConsoleSystem severity; `null` means ignore. `GodotLogMapper.DefaultMap` exposes the built-in policy. The kind identifies the Godot callback, not a universal severity: ScriptError and ShaderError default to Critical, while ErrorStream defaults to Error.

Entries preserve available message and location information. Engine errors use rationale when available or code otherwise, with file/line/function in source metadata. The callback API provides no .NET exception object.

Callbacks may be concurrent. The adapter serializes forwarding, but history notifications still run synchronously on the callback thread. Keep mapping delegates deterministic and non-throwing; consumers marshal their own UI and should not call Godot logging APIs from history callbacks.
