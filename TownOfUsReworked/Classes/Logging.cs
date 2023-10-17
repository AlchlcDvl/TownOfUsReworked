using BepInEx.Logging;

namespace TownOfUsReworked.Classes;

//Adapted from LevelImpostor
public static class Logging
{
    private static ManualLogSource Log;
    public static string SavedLogs;

    public static void Init()
    {
        SavedLogs = "";
        Log = BepInEx.Logging.Logger.CreateLogSource("Reworked");
    }

    private static void LogSomething(object message, LogLevel type)
    {
        Log?.Log(type, message);
        SavedLogs += $"[{type, -7}] {message}\n";
    }

    public static void LogError(object message) => LogSomething(message, LogLevel.Error);

    public static void LogMessage(object message) => LogSomething(message, LogLevel.Message);

    public static void LogFatal(object message) => LogSomething(message, LogLevel.Fatal);

    public static void LogInfo(object message) => LogSomething(message, LogLevel.Info);

    public static void LogWarning(object message) => LogSomething(message, LogLevel.Warning);

    public static void LogDebug(object message) => LogSomething(message, LogLevel.Debug);

    public static void LogNone(object message) => LogSomething(message, LogLevel.None);

    public static void LogAll(object message) => LogSomething(message, LogLevel.All);
}