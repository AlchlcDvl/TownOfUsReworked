using HarmonyLib;

namespace TownOfUsReworked.MCI
{
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.CoStartGameHost))]
    public sealed class OnGameStart
    {
        public static void Prefix(AmongUsClient __instance)
        {
            foreach (var p in __instance.allClients)
                p.IsReady = true;
        }
    }
}