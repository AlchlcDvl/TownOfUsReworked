using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.TrollMod
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.RpcEndGame))]
    public class EndGame
    {
        public static bool Prefix(ShipStatus __instance, [HarmonyArgument(0)] GameOverReason reason)
        {
            foreach (Troll troll in Role.GetRoles(RoleEnum.Troll))
            {
                if (!troll.Killed)
                {
                    troll.Loses();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.TrollLose);
                    writer.Write(troll.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            return true;
        }
    }
}