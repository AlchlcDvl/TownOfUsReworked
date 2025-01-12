using BepInEx.Logging;
using IObject = Il2CppSystem.Object;

// Taken from Reactor.Debugger, it's starting to get annoying having both mine and Reactor's debuggers open at the same time :/
namespace TownOfUsReworked.Patches;

[HarmonyPatch(typeof(Logger))]
public static class RedirectLoggerPatch
{
    private static readonly ManualLogSource _log = BepInEx.Logging.Logger.CreateLogSource("Among Us");

    private static bool Patch(Logger __instance, LogLevel level, IObject message, UObject context)
    {
        if (TownOfUsReworked.BlockBaseGameLogger.Value)
            return false;

        if (!TownOfUsReworked.RedirectLogger.Value)
            return true;

        var finalMessage = "";

        if (__instance.category != Logger.Category.None)
            finalMessage += $"[{__instance.category}] ";

        if (!IsNullEmptyOrWhiteSpace(__instance.tag))
            finalMessage += $"[{__instance.tag}] ";

        if (context != null)
            finalMessage += $"[{context.name} ({context.GetIl2CppType().FullName})]";

        finalMessage += $" {message.ToString()}";
        _log.Log(level, finalMessage);
        return false;
    }

    [HarmonyPatch(nameof(Logger.Debug)), HarmonyPrefix]
    public static bool DebugPatch(Logger __instance, IObject message, UObject context) => Patch(__instance, LogLevel.Debug, message, context);

    [HarmonyPatch(nameof(Logger.Info)), HarmonyPrefix]
    public static bool InfoPatch(Logger __instance, IObject message, UObject context) => Patch(__instance, LogLevel.Info, message, context);

    [HarmonyPatch(nameof(Logger.Warning)), HarmonyPrefix]
    public static bool WarningPatch(Logger __instance, IObject message, UObject context) => Patch(__instance, LogLevel.Warning, message, context);

    [HarmonyPatch(nameof(Logger.Error)), HarmonyPrefix]
    public static bool ErrorPatch(Logger __instance, IObject message, UObject context) => Patch(__instance, LogLevel.Error, message, context);
}