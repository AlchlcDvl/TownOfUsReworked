using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.SectMod
{
    [HarmonyPatch(typeof(GameManager), nameof(GameManager.RpcEndGame))]
    public class EndGame
    {
        public static bool Prefix(GameManager __instance, [HarmonyArgument(0)] GameOverReason reason)
        {
            foreach (var role in Role.GetRoles(SubFaction.Sect))
            {
                if (!Role.SectWin)
                {
                    role.Loses();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.SectLose);
                    writer.Write(role.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            return true;
        }
    }
}