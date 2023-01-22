using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.IntruderMod
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.RpcEndGame))]
    public class EndGame
    {
        public static bool Prefix(ShipStatus __instance, [HarmonyArgument(0)] GameOverReason reason)
        {
            foreach (var role in Role.GetRoles(Faction.Intruder))
            {
                if (!Role.IntruderWin)
                {
                    role.Loses();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.IntruderLose);
                    writer.Write(role.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            return true;
        }
    }
}