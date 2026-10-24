# Quick Start

In a running Godot .NET project, create a `ConsoleManager` and register `new GodotLogAttachment(manager)`. Keep the attachment for the desired logging lifetime and dispose it before the engine shuts down. Godot callbacks can arrive concurrently on worker threads; do not log back through Godot from these callbacks, because that can recurse.
