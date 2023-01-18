using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GlitchMod
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.RpcEndGame))]
    public class EndGame
    {
        public static bool Prefix(ShipStatus __instance, [HarmonyArgument(0)] GameOverReason reason)
        {
            foreach (Glitch gli in Role.GetRoles(RoleEnum.Glitch))
            {
                if (!Role.AllNeutralsWin && !gli.GlitchWins && !Role.NKWins)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GlitchLose, SendOption.Reliable, -1);
                    writer.Write(gli.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    gli.Loses();
                }
            }

            return true;
        }
    }
}