using System;
using System.Collections.Generic;
using HarmonyLib;

namespace TownOfUsReworked.Patches
{
    public class DeadPlayer
    {
        public byte KillerId { get; set; }
        public byte PlayerId { get; set; }
        public DateTime KillTime { get; set; }
    }
    
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