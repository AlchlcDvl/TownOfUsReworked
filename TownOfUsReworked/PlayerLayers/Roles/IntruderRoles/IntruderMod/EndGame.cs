using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.IntruderMod
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.RpcEndGame))]
    public class EndGame
    {
        public static bool Prefix(ShipStatus __instance, [HarmonyArgument(0)] GameOverReason reason)
        {
            if (reason == GameOverReason.HumansByVote || reason == GameOverReason.HumansByTask)
                return true;

            foreach (var role in Role.AllRoles)
            {
                if (role.Faction == Faction.Intruder && !role.IsRecruit)
                    role.Loses();
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte) CustomRPC.IntruderLose,
                SendOption.Reliable, -1);
            AmongUsClient.Instance.FinishRpcImmediately(writer);

            return true;
        }
    }
}