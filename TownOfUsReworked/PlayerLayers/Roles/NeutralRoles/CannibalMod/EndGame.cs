using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.CannibalMod
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.RpcEndGame))]
    public class EndGame
    {
        public static bool Prefix(ShipStatus __instance, [HarmonyArgument(0)] GameOverReason reason)
        {
            foreach (Cannibal cann in Role.GetRoles(RoleEnum.Cannibal))
            {
                if (cann.EatNeed > 0)
                {
                    cann.Loses();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.CannibalLose);
                    writer.Write(cann.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            return true;
        }
    }
}