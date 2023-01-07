using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.PlaguebearerMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
    public class BodyReport
    {
        private static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] GameData.PlayerInfo info)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;
            
            if (CustomGameOptions.PlaguebearerOn == 0)
                return;
            
            if (info == null)
                return;
            
            bool pbflag = false;
            
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(RoleEnum.Plaguebearer))
                    pbflag = true;
            }

            if (!pbflag)
                return;

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.PlayerId == info.Object.PlayerId)
                {
                    if (PlayerControl.LocalPlayer.IsInfected() || player.IsInfected())
                    {
                        foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer))
                            ((Plaguebearer)pb).RpcSpreadInfection(PlayerControl.LocalPlayer, player);
                    }
                }
            }
        }
    }
}