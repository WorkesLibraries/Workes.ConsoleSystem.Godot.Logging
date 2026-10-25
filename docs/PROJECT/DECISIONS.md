# Decisions

Append long-lived decisions using the next `D-NNN` identifier. Preserve superseded entries.

### D-001: Use an explicit disposable Godot logger attachment

#### Context
Godot logging should feed ConsoleSystem without adding engine dependencies to the core or changing the host's normal output.

#### Decision
Register an internal Godot custom logger through `GodotLogAttachment`; unregister it and release the manager on disposal.

#### Reasoning
Godot's logger extension point provides callbacks while a lifetime handle keeps registration under host control.

#### Consequences
Create the attachment after runtime startup and dispose before shutdown. Earlier messages cannot be captured; the caller owns the manager.

### D-002: Map callback kinds and preserve only supplied metadata

#### Context
Godot exposes standard streams and structured engine errors, but does not provide a .NET exception object in these callbacks.

#### Decision
Map `GodotLogKind` to ConsoleSystem levels with a built-in default and optional custom mapper; null ignores. Preserve available error location as source metadata and do not invent exception data.

#### Reasoning
Severity remains an adapter policy and recorded metadata reflects what the source API actually supplied.

#### Consequences
Callbacks may be concurrent and are serialized, but no main-thread guarantee is made. Consumers marshal UI work themselves.
