using BepInEx.Logging;

namespace TownOfUsReworked.Classes;

// Adapted from LevelImpostor
public static class Logging
{
    public static ManualLogSource Log { get; set; }
    public static string SavedLogs { get; set; } = "";

    private static void LogSomething(object message, LogLevel type)
    {
        message ??= "message is null";
        Log?.Log(type, message);
        SavedLogs += $"[{type, -7}] {message}\n";
    }

    public static void Error(object message) => LogSomething(message, LogLevel.Error);

    public static void Message(object message) => LogSomething(message, LogLevel.Message);

    public static void Fatal(object message) => LogSomething(message, LogLevel.Fatal);

    public static void Info(object message) => LogSomething(message, LogLevel.Info);

    public static void Warning(object message) => LogSomething(message, LogLevel.Warning);
}