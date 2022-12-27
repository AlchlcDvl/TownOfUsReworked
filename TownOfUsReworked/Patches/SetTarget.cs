using HarmonyLib;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.SetTarget))]
    public class SetTarget
    {
        private static PlayerControl Target;

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static class Update
        {
            public static void Postfix(HudManager __instance)
            {
                if (PlayerControl.AllPlayerControls.Count <= 1)
                    return;

                var player = PlayerControl.LocalPlayer;

                if (player == null)
                    return;

                if (player.Data == null)
                    return;

                if (__instance.KillButton == null)
                    return;

                if (Role.GetRole(player) == null)
                    return;

                Utils.SetTarget(ref Target, __instance.KillButton, float.NaN);
            }
        }
    }
}