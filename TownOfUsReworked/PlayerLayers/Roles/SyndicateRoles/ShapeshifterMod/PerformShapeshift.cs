using HarmonyLib;
using Hazel;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Data;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.ShapeshifterMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformShapeshift
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Shapeshifter))
                return true;

            var role = Role.GetRole<Shapeshifter>(PlayerControl.LocalPlayer);

            if (__instance == role.ShapeshiftButton)
            {
                if (role.ShapeshiftTimer() != 0f)
                    return false;

                if (role.HoldsDrive)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.Shapeshift);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.TimeRemaining = CustomGameOptions.ShapeshiftDuration;
                    role.Shapeshift();
                    Utils.Shapeshift();
                }
                else
                {
                    var targets = PlayerControl.AllPlayerControls.ToArray().Where(x => !(x == role.ShapeshiftPlayer1 || x == role.ShapeshiftPlayer2 || (x == role.Player &&
                        !CustomGameOptions.WarpSelf) || (x.Data.IsDead && Utils.BodyById(x.PlayerId) == null))).ToList();

                    if (role.ShapeshiftPlayer1 == null)
                        role.ShapeshiftMenu1.Open(targets);
                    else if (role.ShapeshiftPlayer2 == null)
                        role.ShapeshiftMenu2.Open(targets);
                    else
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                        writer.Write((byte)ActionsRPC.Shapeshift);
                        writer.Write(PlayerControl.LocalPlayer.PlayerId);
                        writer.Write(role.ShapeshiftPlayer1.PlayerId);
                        writer.Write(role.ShapeshiftPlayer2.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        role.TimeRemaining = CustomGameOptions.ShapeshiftDuration;
                        role.Shapeshift();
                    }
                }

                return false;
            }

            return true;
        }
    }
}