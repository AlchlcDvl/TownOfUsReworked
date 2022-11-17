using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using System.Linq;
using UnityEngine;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.VampireMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    internal class HasMajority
    {
        public static Sprite Sprite => TownOfUsReworked.Arrow;

        private static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(RoleEnum.Dracula))
                return;

            if (__instance.Data.IsDead)
                return;

            var drac = Role.GetRole<Dracula>(__instance);
            var localRole = Role.GetRole(PlayerControl.LocalPlayer);

            if (drac == null)
                return;

            if (drac.HasMaj)
            {
                if (AmongUsClient.Instance.AmHost)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.DracWin, SendOption.Reliable, -1);
                    writer.Write(drac.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                }
            }
        }
    }
}