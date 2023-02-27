using HarmonyLib;
using Hazel;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using System;
using System.Collections.Generic;
using TownOfUsReworked.Objects;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch]
    public class MurderPlayer
    {
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
        public class MurderPlayerPatch
        {
            public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
            {
                Utils.RpcMurderPlayer(__instance, target, !__instance.Is(AbilityEnum.Ninja));
                return false;
            }
        }

        [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
        [HarmonyPriority(Priority.Last)]
        public class DoClickPatch
        {
            public static bool Prefix(KillButton __instance)
            {
                if (__instance.isActiveAndEnabled && __instance.currentTarget && !__instance.isCoolingDown && !PlayerControl.LocalPlayer.Data.IsDead && PlayerControl.LocalPlayer.CanMove)
                {
                    if (AmongUsClient.Instance.AmHost)
                        PlayerControl.LocalPlayer.CheckMurder(__instance.currentTarget);
                    else
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.CheckMurder, SendOption.Reliable);
                        writer.Write(PlayerControl.LocalPlayer.PlayerId);
                        writer.Write(__instance.currentTarget.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                    
                    __instance.SetTarget(null);
                }
                
                return false;
            }
        }
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