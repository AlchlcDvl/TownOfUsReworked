using BepInEx.Logging;

namespace TownOfUsReworked.Managers;

// Adapted from LevelImpostor
public static class LogManager
{
    public static ManualLogSource Log { get; set; }
    public static DiskLogListener DiskLog { get; set; }
    public static string SavedLogs { get; private set; } = "";

    private static void LogSomething(object message, Enum type, ManualLogSource log = null)
    {
        log ??= Log;
        message ??= "message is null";
        SavedLogs += $"[{type, -7}] {message}\n";

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
        var console = $"[{type, -7}:{log?.SourceName,10}] {message}";
        DiskLog.LogWriter.WriteLine(console);
        ConsoleManager.SetConsoleColor(type.GetConsoleColor());
        ConsoleManager.ConsoleStream?.Write(console + Environment.NewLine);
        ConsoleManager.SetConsoleColor(ConsoleColor.Gray);
    }

    public static void Error(object message, ManualLogSource log = null) => LogSomething(message, LogLevel.Error, log);

    public static void Message(object message, ManualLogSource log = null) => LogSomething(message, LogLevel.Message, log);

    public static void Fatal(object message, ManualLogSource log = null) => LogSomething(message, LogLevel.Fatal, log);

    public static void Info(object message, ManualLogSource log = null) => LogSomething(message, LogLevel.Info, log);

    public static void Warning(object message, ManualLogSource log = null) => LogSomething(message, LogLevel.Warning, log);

    public static void Debug(object message, ManualLogSource log = null) => LogSomething(message, LogLevel.Debug, log);

    public static void Critical(object message, ManualLogSource log = null) => LogSomething(message, ReworkedLogLevel.Critical, log);

    public static void Success(object message, ManualLogSource log = null) => LogSomething(message, ReworkedLogLevel.Success, log);

    public static void Failure(object message, ManualLogSource log = null) => LogSomething(message, ReworkedLogLevel.Failure, log);

    public static void Assert(object message, ManualLogSource log = null) => LogSomething(message, ReworkedLogLevel.Assert, log);

    private static ConsoleColor GetConsoleColor(this ReworkedLogLevel type) => type switch // I'd like me even more colors :P
    {
        ReworkedLogLevel.Critical => ConsoleColor.Blue,
        ReworkedLogLevel.Success => ConsoleColor.Green,
        ReworkedLogLevel.Failure => ConsoleColor.Magenta,
        ReworkedLogLevel.Assert => ConsoleColor.DarkYellow,
        _ => ConsoleColor.DarkGray
    };
}