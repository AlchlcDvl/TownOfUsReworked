using BepInEx.Logging;

namespace TownOfUsReworked.Classes;

// Adapted from LevelImpostor
public static class Logging
{
    public static ManualLogSource Log { get; set; }
    public static DiskLogListener DiskLog { get; set; }
    public static string SavedLogs { get; set; } = "";

    private static void LogSomething(object message, LogLevel type)
    {
        message ??= "message is null";
        Log?.Log(type, message);
        SavedLogs += $"[{type, -7}] {message}\n";
    }

    private static void LogSomething(object message, ReworkedLogLevel type)
    {
        message ??= "message was null";
        DiskLog.LogWriter.WriteLine($"[{type, -7}:{"Reworked",10}] {message}");
        ConsoleManager.SetConsoleColor(type.GetConsoleColor());
        ConsoleManager.ConsoleStream?.Write($"[{type,-7}:{"Reworked",10}] {message}{Environment.NewLine}");
        ConsoleManager.SetConsoleColor(ConsoleColor.Gray);
        SavedLogs += $"[{type, -7}] {message}\n";
    }

    public static void Error(object message) => LogSomething(message, LogLevel.Error);

    public static void Message(object message) => LogSomething(message, LogLevel.Message);

    public static void Fatal(object message) => LogSomething(message, LogLevel.Fatal);

    public static void Info(object message) => LogSomething(message, LogLevel.Info);

    public static void Warning(object message) => LogSomething(message, LogLevel.Warning);

    public static void Critical(object message) => LogSomething(message, ReworkedLogLevel.Critical);

    private static ConsoleColor GetConsoleColor(this ReworkedLogLevel type) => type switch // I'd like me even more colors :P
    {
        ReworkedLogLevel.Critical => ConsoleColor.Blue,
        _ => ConsoleColor.DarkGray
    };
}