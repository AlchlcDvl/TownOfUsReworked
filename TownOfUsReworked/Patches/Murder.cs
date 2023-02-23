using System;
using System.Collections.Generic;
using HarmonyLib;
using TownOfUsReworked.Objects;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
    public class Murder
    {
        public static List<DeadPlayer> KilledPlayers = new List<DeadPlayer>();

        public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
        {
            var deadBody = new DeadPlayer
            {
                PlayerId = target.PlayerId,
                KillerId = __instance.PlayerId,
                KillTime = DateTime.UtcNow
            };

            KilledPlayers.Add(deadBody);
        }
    }
}