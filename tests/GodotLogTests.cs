using System;
using NUnit.Framework;
using Workes.ConsoleSystem.Core;
using Workes.ConsoleSystem.Entries;
using Workes.ConsoleSystem.Logging;
using Workes.ConsoleSystem.Godot.Logging;

namespace Workes.ConsoleSystem.Godot.Logging.Tests;

public sealed class GodotLogTests
{
    [TestCase(GodotLogKind.Standard, LogLevel.Information)]
    [TestCase(GodotLogKind.ErrorStream, LogLevel.Error)]
    [TestCase(GodotLogKind.EngineError, LogLevel.Error)]
    [TestCase(GodotLogKind.EngineWarning, LogLevel.Warning)]
    [TestCase(GodotLogKind.ScriptError, LogLevel.Critical)]
    [TestCase(GodotLogKind.ShaderError, LogLevel.Critical)]
    public void DefaultMap_MapsKinds(GodotLogKind kind, LogLevel expected) => Assert.That(GodotLogMapper.DefaultMap(kind), Is.EqualTo(expected));

    [Test]
    public void Forwarder_PreservesMessageAndSource_AndDisposalStopsWrites()
    {
        var manager = new ConsoleManager();
        var forwarder = new GodotLogForwarder(manager, GodotLogMapper.DefaultMap);
        forwarder.Write(GodotLogKind.EngineWarning, "careful", "res://scene.tscn:12");
        forwarder.Dispose();
        forwarder.Write(GodotLogKind.Standard, "after", "Godot");
        var entry = (LogEntry)manager.History.Entries[0];
        Assert.That(entry.Message, Is.EqualTo("careful"));
        Assert.That(entry.Source, Is.EqualTo("res://scene.tscn:12"));
        Assert.That(entry.Level, Is.EqualTo(LogLevel.Warning));
        Assert.That(manager.History.Entries.Count, Is.EqualTo(1));
    }

    [Test]
    public void CustomMapper_CanIgnoreSourceKind()
    {
        var manager = new ConsoleManager();
        using var forwarder = new GodotLogForwarder(manager, kind => kind == GodotLogKind.Standard ? null : LogLevel.Debug);
        forwarder.Write(GodotLogKind.Standard, "ignored", null);
        Assert.That(manager.History.Entries.Count, Is.Zero);
    }
}
