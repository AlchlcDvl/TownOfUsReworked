using HarmonyLib;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.BetterMaps.Airship
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Start))]
    class GameEndedPatch
    {
        public static void Postfix(ShipStatus __instance)
        {
            SpawnInMinigamePatch.GameStarted = false;
        }
    }

    [HarmonyPatch(typeof(HeliSabotageSystem), nameof(HeliSabotageSystem.UpdateHeliSize))]
    class HeliCountDown
    {
        public static void Prefix(HeliSabotageSystem __instance)
        {
            if (__instance.Countdown > CustomGameOptions.CrashTimer)
                __instance.Countdown = CustomGameOptions.CrashTimer;
        }
    }
}
