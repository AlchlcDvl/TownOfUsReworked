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

    public static void LogError(object message) => LogSomething(message, LogLevel.Error);

    public static void LogMessage(object message) => LogSomething(message, LogLevel.Message);

    public static void LogFatal(object message) => LogSomething(message, LogLevel.Fatal);

    public static void LogInfo(object message) => LogSomething(message, LogLevel.Info);

    public static void LogWarning(object message) => LogSomething(message, LogLevel.Warning);
}