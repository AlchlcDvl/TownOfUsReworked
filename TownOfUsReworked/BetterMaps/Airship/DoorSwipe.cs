using HarmonyLib;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.BetterMaps.Airship
{
    [HarmonyPatch(typeof(DoorCardSwipeGame), nameof(DoorCardSwipeGame.Begin))]
    internal class DoorSwipePatch
    {
        private static void Prefix(DoorCardSwipeGame __instance) => __instance.minAcceptedTime = CustomGameOptions.MinDoorSwipeTime;
    }
}