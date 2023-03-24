using HarmonyLib;
using TownOfUsReworked.Classes;
using System;
using System.Collections.Generic;
using TownOfUsReworked.Objects;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
    public static class Murder
    {
        public readonly static List<DeadPlayer> KilledPlayers = new();

        public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
        {
            Utils.RpcMurderPlayer(__instance, target);
            return false;
        }

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