using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.MurdererMod
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.RpcEndGame))]
    public class EndGame
    {
        public static bool Prefix(ShipStatus __instance, [HarmonyArgument(0)] GameOverReason reason)
        {
            foreach (Murderer murd in Role.GetRoles(RoleEnum.Murderer))
            {
                if (!Role.AllNeutralsWin && !Role.NKWins && !murd.MurdWins)
                {
                    murd.Loses();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.MurdererLose);
                    writer.Write(murd.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            return true;
        }
    }
}