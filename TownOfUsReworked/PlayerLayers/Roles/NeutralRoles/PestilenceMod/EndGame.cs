using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.PestilenceMod
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.RpcEndGame))]
    public class EndGame
    {
        public static bool Prefix(ShipStatus __instance, [HarmonyArgument(0)] GameOverReason reason)
        {
            foreach (Pestilence pest in Role.GetRoles(RoleEnum.Pestilence))
            {
                if (!Role.AllNeutralsWin && !Role.NKWins && !pest.PestilenceWins)
                {
                    pest.Loses();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.PestilenceLose);
                    writer.Write(pest.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            return true;
        }
    }
}