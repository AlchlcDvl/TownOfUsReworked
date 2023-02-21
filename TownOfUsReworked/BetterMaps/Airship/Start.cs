using HarmonyLib;
using TownOfUsReworked.CustomOptions;

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
}
