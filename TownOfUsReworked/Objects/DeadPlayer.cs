using System;
using HarmonyLib;

namespace TownOfUsReworked.Objects
{
    [HarmonyPatch]
    public class DeadPlayer
    {
        public byte KillerId;
        public byte PlayerId;
        public DateTime KillTime;
    }
}