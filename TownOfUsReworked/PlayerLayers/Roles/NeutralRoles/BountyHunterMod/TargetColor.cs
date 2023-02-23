using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.BountyHunterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class BHTargetColor
    {
        private static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.BountyHunter))
                return;

            var role = Role.GetRole<BountyHunter>(PlayerControl.LocalPlayer);

            if (role.TargetPlayer == null)
                return;

            if (role.TargetKilled)
                return;

            if (!role.TargetFound && role.UsesLeft > 0)
                return;

            if (role.TargetFound)
            {
                role.TargetPlayer.nameText().color = role.Color;
                role.TargetPlayer.nameText().text += " Φ";
                return;
            }

            if (!(role.TargetPlayer.Data.IsDead || role.TargetPlayer.Data.Disconnected))
                return;

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
            writer.Write((byte)TurnRPC.BHToTroll);
            writer.Write(PlayerControl.LocalPlayer.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            BHToTroll(PlayerControl.LocalPlayer);
        }

        public static void BHToTroll(PlayerControl player)
        {
            var bh = Role.GetRole<BountyHunter>(player);
            var newRole = new Troll(player);
            newRole.RoleHistory.Add(bh);
            newRole.RoleHistory.AddRange(bh.RoleHistory);
            player.RegenTask();
        }
    }
}