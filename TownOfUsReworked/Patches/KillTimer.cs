using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetKillTimer))]
    public static class KillTimer
    {
        public static bool Prefix(PlayerControl __instance, ref float time)
        {
            if (__instance.Data.Role.CanUseKillButton)
            {
                if (CustomGameOptions.IntKillCooldown <= 0f)
                    return false;

                var maxvalue = time > CustomGameOptions.IntKillCooldown ? time + 1f : CustomGameOptions.IntKillCooldown;
                __instance.killTimer = Mathf.Clamp(time, 0, maxvalue);
                DestroyableSingleton<HudManager>.Instance.KillButton.SetCoolDown(__instance.killTimer, maxvalue);
            }

            return false;
        } 
    }
}