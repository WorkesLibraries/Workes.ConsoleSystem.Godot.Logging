using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Workes.ConsoleSystem.Core;
using Workes.ConsoleSystem.Entries;
using Workes.ConsoleSystem.Godot.Logging;
using Workes.ConsoleSystem.Logging;

public partial class ConsumerTest : Node
{
    public override void _Ready()
    {
        var manager = new ConsoleManager();
        using var attachment = new GodotLogAttachment(manager);

        GD.Print("CONSUMER_STANDARD_MARKER");
        GD.PushWarning("CONSUMER_WARNING_MARKER");
        GD.PushError("CONSUMER_ERROR_MARKER");
        Task.Run(() => GD.Print("CONSUMER_BACKGROUND_MARKER")).GetAwaiter().GetResult();

        var entries = manager.History.Entries.OfType<LogEntry>().ToArray();
        GD.Print("CONSUMER_CAPTURE_COUNT: " + entries.Length);
        foreach (var entry in entries)
            GD.Print("CONSUMER_CAPTURED: " + entry.Level + " | " + entry.Source + " | " + entry.Message);
        Assert(entries.Any(entry => entry.Message.Contains("CONSUMER_STANDARD_MARKER") && entry.Level == LogLevel.Information), "Standard output should be Information.");
        Assert(entries.Any(entry => entry.Message.Contains("CONSUMER_WARNING_MARKER") && entry.Level == LogLevel.Warning), "Warnings should be Warning.");
        Assert(entries.Any(entry => entry.Message.Contains("CONSUMER_ERROR_MARKER") && entry.Level == LogLevel.Error), "Godot error messages should be Error.");
        Assert(entries.Any(entry => entry.Message.Contains("CONSUMER_BACKGROUND_MARKER") && entry.Level == LogLevel.Information), "Background-thread output should be captured.");

        attachment.Dispose();
        int countAfterDetach = manager.History.Entries.Count;
        GD.Print("CONSUMER_AFTER_DISPOSE_MARKER");
        Assert(manager.History.Entries.Count == countAfterDetach, "Output after disposal should not be captured.");

        GD.Print("GODOT_CONSUMER_TEST_PASS");
        GetTree().Quit();
    }

    private static void Assert(bool condition, string message)
    {
        if (condition) return;
        Console.Error.WriteLine("GODOT_CONSUMER_TEST_FAIL: " + message);
        throw new InvalidOperationException(message);
    }
}
