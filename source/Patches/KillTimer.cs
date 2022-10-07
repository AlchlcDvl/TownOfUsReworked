using HarmonyLib;
using UnityEngine;

namespace TownOfUs.Patches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetKillTimer))]
    public static class KillTimer
    {
        public static bool Prefix(PlayerControl __instance, ref float time)
        {
            if (__instance.Data.Role.CanUseKillButton)
            {
                if (PlayerControl.GameOptions.KillCooldown <= 0f)
                {
                    return false;
                }

                var maxvalue = time > PlayerControl.GameOptions.killCooldown ? time + 1f : PlayerControl.GameOptions.killCooldown;
                __instance.killTimer = Mathf.Clamp(time, 0, maxvalue);
                DestroyableSingleton<HudManager>.Instance.KillButton.SetCoolDown(__instance.killTimer, maxvalue);
            }

            return false;
        } 
    }
}