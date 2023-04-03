using HarmonyLib;

namespace TownOfUsReworked.MultiClientInstancing
{
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.CoStartGameHost))]
    public sealed class OnGameStart
    {
        public static void Prefix(AmongUsClient __instance)
        {
            if (TownOfUsReworked.MCIActive)
            {
                foreach (var p in __instance.allClients)
                    p.IsReady = true;
            }
        }
    }
}