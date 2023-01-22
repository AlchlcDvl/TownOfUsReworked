using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using Hazel;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.SidekickMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class Promote
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Sidekick))
                return;
            
            var role = Role.GetRole<Sidekick>(PlayerControl.LocalPlayer);
            var isDead = role.Player.Data.IsDead;

            if (role.CanPromote && !isDead)
            {
                role.TurnRebel();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable, -1);
                writer.Write((byte)TurnRPC.TurnRebel);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }
    }
}