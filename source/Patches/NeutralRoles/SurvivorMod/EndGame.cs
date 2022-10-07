using HarmonyLib;
using Hazel;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.SurvivorMod
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.RpcEndGame))]
    public class EndGame
    {
        public static bool Prefix(ShipStatus __instance, [HarmonyArgument(0)] GameOverReason reason)
        {
            if (reason != GameOverReason.HumansByVote && reason != GameOverReason.HumansByTask)
            {
                foreach (var role in Role.AllRoles)
                    if (role.RoleType == RoleEnum.Survivor && !role.Player.Data.IsDead && !role.Player.Data.Disconnected)
                    {
                        ((Survivor)role).Wins();
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                            (byte)CustomRPC.SurvivorWin, SendOption.Reliable, -1);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        return true;
                    }
            }
            else
            {
                foreach (var role in Role.AllRoles)
                    if (role.RoleType == RoleEnum.Survivor && (role.Player.Data.IsDead || role.Player.Data.Disconnected))
                    {
                        ((Survivor)role).Loses();

                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                            (byte)CustomRPC.SurvivorLose, SendOption.Reliable, -1);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        return true;
                    }
            }
            return true;
        }
    }
}