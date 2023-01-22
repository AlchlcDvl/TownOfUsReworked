using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.CryomaniacMod
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.RpcEndGame))]
    public class EndGame
    {
        public static bool Prefix(ShipStatus __instance, [HarmonyArgument(0)] GameOverReason reason)
        {
            foreach (Cryomaniac cryo in Role.GetRoles(RoleEnum.Cryomaniac))
            {
                if (!Role.AllNeutralsWin && !Role.NKWins && !cryo.CryoWins)
                {
                    cryo.Loses();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.CryomaniacLose);
                    writer.Write(cryo.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            return true;
        }
    }
}