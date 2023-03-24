using HarmonyLib;
using System;

namespace TownOfUsReworked.MCI
{
    [HarmonyPatch(typeof(SpawnInMinigame), nameof(SpawnInMinigame.Begin))]
    public sealed class AirshipSpawn
    {
        public static void Postfix(SpawnInMinigame __instance)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!player.Data.PlayerName.Contains("Robot"))
                    continue;

                var rand = new Random().Next(0, __instance.Locations.Count);
                player.gameObject.SetActive(true);
                player.NetTransform.RpcSnapTo(__instance.Locations[rand].Location);
            }
        }
    }
}