using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.VampireMod
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.RpcEndGame))]
    public class EndGame
    {
        public static bool Prefix(ShipStatus __instance, [HarmonyArgument(0)] GameOverReason reason)
        {
            if (reason != GameOverReason.HumansByVote && reason != GameOverReason.HumansByTask)
                return true;

            foreach (var role in Role.AllRoles)
            {
                if (role.RoleType == RoleEnum.Dracula)
                    ((Dracula) role).Loses();
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.DracLose,
                SendOption.Reliable, -1);

            foreach (var role in Role.AllRoles)
            {
                if (role.RoleType == RoleEnum.Vampire)
                    ((Vampire) role).Loses();
            }

            var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.VampLose,
                SendOption.Reliable, -1);

            AmongUsClient.Instance.FinishRpcImmediately(writer);
            AmongUsClient.Instance.FinishRpcImmediately(writer2);

            return true;
        }
    }
}