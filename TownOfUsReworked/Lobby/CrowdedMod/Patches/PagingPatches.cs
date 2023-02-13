using HarmonyLib;
using TownOfUsReworked.Lobby.CrowdedMod.Components;

namespace TownOfUsReworked.Lobby.CrowdedMod.Patches
{
    internal static class PagingPatches 
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        public static class MeetingHudStartPatch
        {
            public static void Postfix(MeetingHud __instance)
            {
                __instance.gameObject.AddComponent<MeetingHudPagingBehaviour>().meetingHud = __instance;
            }
        }

        [HarmonyPatch(typeof(ShapeshifterMinigame), nameof(ShapeshifterMinigame.Begin))]
        public static class ShapeshifterMinigameBeginPatch
        {
            public static void Postfix(ShapeshifterMinigame __instance)
            {
                __instance.gameObject.AddComponent<ShapeShifterPagingBehaviour>().shapeshifterMinigame = __instance;
            }
        }
    }
}
