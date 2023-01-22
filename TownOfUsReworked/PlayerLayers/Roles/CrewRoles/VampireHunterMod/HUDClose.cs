using System;
using HarmonyLib;
using TownOfUsReworked.Enums;
using Object = UnityEngine.Object;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using Hazel;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.VampireHunterMod
{
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDClose
    {
        public static void Postfix(Object __instance)
        {
            if (ExileController.Instance == null || __instance != ExileController.Instance.gameObject)
                return;
            
            var VampsExist = false;

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(SubFaction.Undead))
                {
                    VampsExist = true;
                    break;
                }
            }

            if (!VampsExist)
            {
                foreach (VampireHunter vh in Role.GetRoles(RoleEnum.VampireHunter))
                {
                    vh.TurnVigilante();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable, -1);
                    writer.Write((byte)TurnRPC.TurnVigilante);
                    writer.Write(vh.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
            else
            {
                foreach (VampireHunter vh in Role.GetRoles(RoleEnum.VampireHunter))
                    vh.LastStaked = DateTime.UtcNow;
            }
        }
    }
}