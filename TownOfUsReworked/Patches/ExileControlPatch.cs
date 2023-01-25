using HarmonyLib;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
    [HarmonyPriority(Priority.First)]
    class ExileControllerPatch
    {
        public static ExileController lastExiled;
        
        public static void Prefix(ExileController __instance)
        {
            lastExiled = __instance;
        }
    }
}