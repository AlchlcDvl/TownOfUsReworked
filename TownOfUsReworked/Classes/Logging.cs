using BepInEx.Logging;

namespace TownOfUsReworked.Classes;

// Adapted from LevelImpostor
public static class Logging
{
    public static ManualLogSource Log { get; set; }
    public static DiskLogListener DiskLog { get; set; }
    public static string SavedLogs { get; set; } = "";

    private static void LogSomething(object message, Enum type)
    {
        message ??= "message is null";
        SavedLogs += $"[{type, -7}] {message}\n";

        if (type is LogLevel bll)
            BIELog(message, bll);
        else if (type is ReworkedLogLevel rll)
            RewLog(message, rll);
    }

    private static void BIELog(object message, LogLevel type) => Log?.Log(type, message);

    private static void RewLog(object message, ReworkedLogLevel type)
    {
        var console = $"[{type, -7}:{"Reworked",10}] {message}";
        DiskLog.LogWriter.WriteLine(console);
        ConsoleManager.SetConsoleColor(type.GetConsoleColor());
        ConsoleManager.ConsoleStream?.Write($"{console}{Environment.NewLine}");
        ConsoleManager.SetConsoleColor(ConsoleColor.Gray);
    }

    public static void Error(object message) => LogSomething(message, LogLevel.Error);

    public static void Message(object message) => LogSomething(message, LogLevel.Message);

    public static void Fatal(object message) => LogSomething(message, LogLevel.Fatal);

    public static void Info(object message) => LogSomething(message, LogLevel.Info);

    public static void Warning(object message) => LogSomething(message, LogLevel.Warning);

    public static void Critical(object message) => LogSomething(message, ReworkedLogLevel.Critical);

    public static void Success(object message) => LogSomething(message, ReworkedLogLevel.Success);

    public static void Failure(object message) => LogSomething(message, ReworkedLogLevel.Failure);

    private static ConsoleColor GetConsoleColor(this ReworkedLogLevel type) => type switch // I'd like me even more colors :P
    {
        ReworkedLogLevel.Critical => ConsoleColor.Blue,
        ReworkedLogLevel.Success => ConsoleColor.Green,
        ReworkedLogLevel.Failure => ConsoleColor.Magenta,
        _ => ConsoleColor.DarkGray
    };
}