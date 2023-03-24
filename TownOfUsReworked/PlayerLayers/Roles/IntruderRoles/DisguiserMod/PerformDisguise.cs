using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.DisguiserMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformDisguise
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Disguiser))
                return true;

            var role = Role.GetRole<Disguiser>(PlayerControl.LocalPlayer);

            if (__instance == role.DisguiseButton)
            {
                if (role.DisguiseTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestTarget))
                    return false;

                if (role.ClosestTarget == role.MeasuredPlayer)
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestTarget);

                if (interact[3])
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.Disguise);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(role.MeasuredPlayer.PlayerId);
                    writer.Write(role.ClosestTarget.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.TimeRemaining = CustomGameOptions.DisguiseDuration;
                    role.TimeRemaining2 = CustomGameOptions.TimeToDisguise;
                    role.DisguisedPlayer = role.ClosestTarget;
                    role.Delay();
                }
                else if (interact[0])
                    role.LastDisguised = DateTime.UtcNow;
                else if (interact[1])
                    role.LastDisguised.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }
            else if (__instance == role.MeasureButton)
            {
                if (role.MeasureTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestTarget))
                    return false;

                if (role.ClosestTarget == role.MeasuredPlayer)
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestTarget);

                if (interact[3])
                    role.MeasuredPlayer = role.ClosestTarget;

                if (interact[0])
                    role.LastMeasured = DateTime.UtcNow;
                else if (interact[1])
                    role.LastMeasured.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }

            return true;
        }
    }
}