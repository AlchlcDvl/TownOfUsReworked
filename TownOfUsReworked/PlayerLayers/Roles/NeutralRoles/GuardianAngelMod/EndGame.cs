using HarmonyLib;
using TownOfUsReworked.Enums;
using Hazel;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GuardianAngelMod
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.RpcEndGame))]
    public class EndGame
    {
        public static bool Prefix(ShipStatus __instance, [HarmonyArgument(0)] GameOverReason reason)
        {
            foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel))
            {
                if (ga.TargetPlayer == null)
                    continue;
                    
                if (ga.TargetAlive)
                {
                    ga.Wins();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.GuardianAngelWin);
                    writer.Write(ga.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
                else
                {
                    ga.Loses();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.GuardianAngelLose);
                    writer.Write(ga.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            return true;
        }
    }
}