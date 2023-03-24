using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.SidekickMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class Promote
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Sidekick))
                return;

            var role = Role.GetRole<Sidekick>(PlayerControl.LocalPlayer);

            if (role.CanPromote && !role.Player.Data.IsDead)
            {
                role.TurnRebel();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                writer.Write((byte)TurnRPC.TurnRebel);
                writer.Write(role.Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }
    }
}