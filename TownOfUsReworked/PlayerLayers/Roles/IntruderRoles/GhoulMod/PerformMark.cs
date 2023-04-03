using HarmonyLib;
using Hazel;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.GhoulMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformMark
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Ghoul))
                return true;

            var role = Role.GetRole<Ghoul>(PlayerControl.LocalPlayer);

            if (__instance == role.MarkButton)
            {
                if (role.MarkTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestMark))
                    return false;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.Mark);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(role.ClosestMark.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                role.MarkedPlayer = role.ClosestMark;
                return false;
            }

            return true;
        }
    }
}