using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Abilities.UnderdogMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetKillTimer))]
    public static class PatchKillTimer
    {
        public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] float time)
        {
            var ability = Ability.GetAbility(__instance);

            if (ability.AbilityType != AbilityEnum.Underdog)
                return true;

            var maxTimer = ((Underdog)ability).MaxTimer();
            __instance.killTimer = Mathf.Clamp(time, 0, maxTimer);
            HudManager.Instance.KillButton.SetCoolDown(__instance.killTimer, maxTimer);
            return false;
        }
    }
}
