using HarmonyLib;
using TownOfUsReworked.Classes;
using Hazel;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.MafiosoMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class Promote
    {
        public static void Postfix()
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Mafioso))
                return;

            var role = Role.GetRole<Mafioso>(PlayerControl.LocalPlayer);

            if (role.CanPromote && !role.Player.Data.IsDead)
            {
                role.TurnGodfather();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                writer.Write((byte)TurnRPC.TurnGodfather);
                writer.Write(role.Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }
    }
}