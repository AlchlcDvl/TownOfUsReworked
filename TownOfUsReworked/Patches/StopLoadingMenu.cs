
using HarmonyLib;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(SplashManager), nameof(SplashManager.Update))]
    public static class StopLoadingMainMenu
    {
        public static bool Prefix()
        {
            return !BepInExUpdater.UpdateRequired;
        }
    }
}