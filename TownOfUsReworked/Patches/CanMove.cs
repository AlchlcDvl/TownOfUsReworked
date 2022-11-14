using HarmonyLib;

namespace TownOfUsReworked.Patches
{
    public static class CanMove
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CanMove), MethodType.Getter)]
        internal static class CanMovePatch
        {
            public static bool Prefix(PlayerControl __instance, ref bool __result)
            {
                __result = __instance.moveable && !Minigame.Instance && !__instance.shapeshifting && (!DestroyableSingleton<HudManager>.InstanceExists
                    | !DestroyableSingleton<HudManager>.Instance.Chat.IsOpen && !DestroyableSingleton<HudManager>.Instance.KillOverlay.IsOpen
                    && !DestroyableSingleton<HudManager>.Instance.GameMenu.IsOpen) && (!MapBehaviour.Instance || !MapBehaviour.Instance.IsOpenStopped)
                    && !MeetingHud.Instance && !PlayerCustomizationMenu.Instance && !ExileController.Instance && !IntroCutscene.Instance;

                return false;
            }
        }
    }
}