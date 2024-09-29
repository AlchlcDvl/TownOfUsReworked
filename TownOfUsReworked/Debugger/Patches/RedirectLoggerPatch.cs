using BepInEx.Logging;

// Taken from Reactor.Debugger, it start to get annoying having both mine and Reactor's debuggers open at the same time :/
namespace TownOfUsReworked.Debugger.Patches;

[HarmonyPatch(typeof(Logger))]
public static class RedirectLoggerPatch
{
    private static readonly ManualLogSource _log = BepInEx.Logging.Logger.CreateLogSource("Among Us");

    private static bool Enabled => TownOfUsReworked.RedirectLogger.Value;

    private static void Log(Logger logger, Logger.Level level, Il2CppSystem.Object message, UObject context = null)
    {
        var finalMessage = new StringBuilder();

        if (logger.category != Logger.Category.None)
            finalMessage.Append($"[{logger.category}] ");

        if (!string.IsNullOrEmpty(logger.tag))
            finalMessage.Append($"[{logger.tag}] ");

        if (context != null)
            finalMessage.Append($"[{context.name} ({context.GetIl2CppType().FullName})]");

        finalMessage.Append($" {message.ToString()}");

        _log.Log(level switch
        {
            Logger.Level.Debug => LogLevel.Debug,
            Logger.Level.Error => LogLevel.Error,
            Logger.Level.Warning => LogLevel.Warning,
            Logger.Level.Info => LogLevel.Info,
            _ => throw new ArgumentOutOfRangeException(nameof(level), level, null),
        }, finalMessage);
    }

    [HarmonyPatch(nameof(Logger.Debug))]
    [HarmonyPrefix]
    public static bool DebugPatch(Logger __instance, Il2CppSystem.Object message, UObject context)
    {
        if (Enabled)
            Log(__instance, Logger.Level.Debug, message, context);

        return !Enabled;
    }

    [HarmonyPatch(nameof(Logger.Info))]
    [HarmonyPrefix]
    public static bool InfoPatch(Logger __instance, Il2CppSystem.Object message, UObject context)
    {
        if (Enabled)
            Log(__instance, Logger.Level.Info, message, context);

        return !Enabled;
    }

    [HarmonyPatch(nameof(Logger.Warning))]
    [HarmonyPrefix]
    public static bool WarningPatch(Logger __instance, Il2CppSystem.Object message, UObject context)
    {
        if (Enabled)
            Log(__instance, Logger.Level.Warning, message, context);

        return !Enabled;
    }

    [HarmonyPatch(nameof(Logger.Error))]
    [HarmonyPrefix]
    public static bool ErrorPatch(Logger __instance, Il2CppSystem.Object message, UObject context)
    {
        if (Enabled)
            Log(__instance, Logger.Level.Error, message, context);

        return !Enabled;
    }
}