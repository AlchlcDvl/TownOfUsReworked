using System;
using HarmonyLib;

namespace TownOfUsReworked.Objects
{
    [HarmonyPatch]
    public class DeadPlayer
    {
        public byte KillerId { get; set; }
        public byte PlayerId { get; set; }
        public DateTime KillTime { get; set; }
    }
}