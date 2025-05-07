using BepInEx.Logging;

// Taken from Reactor.Debugger, it's starting to get annoying having both mine and Reactor's debuggers open at the same time :/
namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(Logger))]
public static class RedirectLoggerPatch1
{
    private static readonly ManualLogSource Log = BepInEx.Logging.Logger.CreateLogSource("Among Us");

    private static bool Patch(Logger __instance, LogLevel level, Il2CppStringArray path, IObject message, UObject context)
    {
        if (TownOfUsReworked.BlockBaseGameLogger.Value)
            return false;

        if (!TownOfUsReworked.RedirectLogger.Value)
            return true;

        var finalMessage = "";

        if (__instance.category != Logger.Category.None)
            finalMessage += $"[{__instance.category}] ";

        __instance.subCategories?.Do(x => finalMessage += $"[{x}] ");
        path?.Do(x => finalMessage += $"[{x}] ");

        if (context)
            finalMessage += $"[{context.name} ({context.GetIl2CppType().FullName})]";

        finalMessage += $" {message.ToString()}";
        Log.Log(level, finalMessage);
        return false;
    }

    [HarmonyPatch(nameof(Logger.Debug), typeof(Il2CppStringArray), typeof(IObject), typeof(UObject)), HarmonyPrefix]
    public static bool DebugPatch1(Logger __instance, Il2CppStringArray path, IObject message, UObject context) => Patch(__instance, LogLevel.Debug, path, message, context);

    [HarmonyPatch(nameof(Logger.Info), typeof(Il2CppStringArray), typeof(IObject), typeof(UObject)), HarmonyPrefix]
    public static bool InfoPatch1(Logger __instance, Il2CppStringArray path, IObject message, UObject context) => Patch(__instance, LogLevel.Info, path, message, context);

    [HarmonyPatch(nameof(Logger.Warning), typeof(Il2CppStringArray), typeof(IObject), typeof(UObject)), HarmonyPrefix]
    public static bool WarningPatch1(Logger __instance, Il2CppStringArray path, IObject message, UObject context) => Patch(__instance, LogLevel.Warning, path, message, context);

    [HarmonyPatch(nameof(Logger.Error), typeof(Il2CppStringArray), typeof(IObject), typeof(UObject)), HarmonyPrefix]
    public static bool ErrorPatch1(Logger __instance, Il2CppStringArray path, IObject message, UObject context) => Patch(__instance, LogLevel.Error, path, message, context);

    [HarmonyPatch(nameof(Logger.Debug), typeof(IObject), typeof(UObject)), HarmonyPrefix]
    public static bool DebugPatch2(Logger __instance, IObject message, UObject context) => Patch(__instance, LogLevel.Debug, null, message, context);

    [HarmonyPatch(nameof(Logger.Info), typeof(IObject), typeof(UObject)), HarmonyPrefix]
    public static bool InfoPatch2(Logger __instance, IObject message, UObject context) => Patch(__instance, LogLevel.Info, null, message, context);

    [HarmonyPatch(nameof(Logger.Warning), typeof(IObject), typeof(UObject)), HarmonyPrefix]
    public static bool WarningPatch2(Logger __instance, IObject message, UObject context) => Patch(__instance, LogLevel.Warning, null, message, context);

    [HarmonyPatch(nameof(Logger.Error), typeof(IObject), typeof(UObject)), HarmonyPrefix]
    public static bool ErrorPatch2(Logger __instance, IObject message, UObject context) => Patch(__instance, LogLevel.Error, null, message, context);
}

public static class RedirectLoggerPatch2
{
    private static readonly ManualLogSource Log = BepInEx.Logging.Logger.CreateLogSource("UnityLog");

    public static void UnityLog(string msg, string stackTrace, LogType type)
    {
        if (!TownOfUsReworked.LogFromUnity.Value)
            return;

        var message = $"Unity Stack Trace:\n{msg}\n{stackTrace}";

        switch (type)
        {
            case LogType.Error:
            {
                Error(message, Log);
                break;
            }
            case LogType.Warning:
            {
                Warning(message, Log);
                break;
            }
            case LogType.Log:
            {
                Debug(message, Log);
                break;
            }
            case LogType.Exception:
            {
                Failure(message, Log);
                break;
            }
            default:
            {
                Assert(message, Log);
                break;
            }
        }
    }
}