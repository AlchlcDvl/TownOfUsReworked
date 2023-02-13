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
}
