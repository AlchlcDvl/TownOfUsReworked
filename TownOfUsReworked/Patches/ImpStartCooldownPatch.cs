using HarmonyLib;
using System;
using UnityEngine;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetKillTimer))]
    public static class PatchKillTimer
    {
        [HarmonyPriority(Priority.First)]
        public static void Prefix(PlayerControl __instance, ref float time)
        {
            if (GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown > 10 && __instance.Data.IsImpostor() && time <= 10 && Math.Abs(__instance.killTimer - time) >
                2 * Time.deltaTime)
                time = CustomGameOptions.InitialCooldowns;
        }
    }
}