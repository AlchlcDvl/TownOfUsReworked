using BepInEx.Logging;

namespace TownOfUsReworked.Managers;

// Adapted from LevelImpostor
public static class LogManager
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public static ManualLogSource Log;
    public static DiskLogListener DiskLog;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public static readonly StringBuilder SavedLogs = new();

    private static void LogSomething<T>(object message, T type, ManualLogSource? log = null) where T : struct, Enum
    {
        log ??= Log;
        message ??= "message is null";
        SavedLogs.AppendLine($"[{type, -7}] {message}");

        switch (type)
        {
            case LogLevel bll:
            {
                BieLog(message, bll, log);
                break;
            }
            case ReworkedLogLevel rll:
            {
                RewLog(message, rll, log);
                break;
            }
        }
    }

    private static void BieLog(object message, LogLevel type, ManualLogSource log) => log.Log(type, message);

    private static void RewLog(object message, ReworkedLogLevel type, ManualLogSource log)
    {
        var console = $"[{type, -7}:{log?.SourceName, 10}] {message}";
        DiskLog.LogWriter.WriteLine(console);
        ConsoleManager.SetConsoleColor(type.GetConsoleColor());
        ConsoleManager.ConsoleStream?.Write(console + Environment.NewLine);
        ConsoleManager.SetConsoleColor(ConsoleColor.Gray);
    }

    public static void Error(object message, ManualLogSource? log = null) => LogSomething(message, LogLevel.Error, log);

    public static void Message(object message, ManualLogSource? log = null) => LogSomething(message, LogLevel.Message, log);

    public static void Fatal(object message, ManualLogSource? log = null) => LogSomething(message, LogLevel.Fatal, log);

    public static void Info(object message, ManualLogSource? log = null) => LogSomething(message, LogLevel.Info, log);

    public static void Warning(object message, ManualLogSource? log = null) => LogSomething(message, LogLevel.Warning, log);

    public static void Debug(object message, ManualLogSource? log = null) => LogSomething(message, LogLevel.Debug, log);

    public static void Critical(object message, ManualLogSource? log = null) => LogSomething(message, ReworkedLogLevel.Critical, log);

    public static void Success(object message, ManualLogSource? log = null) => LogSomething(message, ReworkedLogLevel.Success, log);

    public static void Failure(object message, ManualLogSource? log = null) => LogSomething(message, ReworkedLogLevel.Failure, log);

    public static void Assert(object message, ManualLogSource? log = null) => LogSomething(message, ReworkedLogLevel.Assert, log);

    private static ConsoleColor GetConsoleColor(this ReworkedLogLevel type) => type switch // I'd like me even more colors :P
    {
        ReworkedLogLevel.Critical => ConsoleColor.Blue,
        ReworkedLogLevel.Success => ConsoleColor.Green,
        ReworkedLogLevel.Failure => ConsoleColor.Magenta,
        ReworkedLogLevel.Assert => ConsoleColor.DarkYellow,
        _ => ConsoleColor.DarkGray
    };
}