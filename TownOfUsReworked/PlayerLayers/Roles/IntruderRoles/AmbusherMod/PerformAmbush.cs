using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using System;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.AmbusherMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformAmbush
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Ambusher))
                return true;

            var role = Role.GetRole<Ambusher>(PlayerControl.LocalPlayer);

            if (__instance == role.AmbushButton)
            {
                if (role.AmbushTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestAmbush))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestAmbush);

                if (interact[3])
                {
                    role.TimeRemaining = CustomGameOptions.AmbushDuration;
                    role.AmbushedPlayer = role.ClosestAmbush;
                    role.Ambush();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.Ambush);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(role.AmbushedPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
                else if (interact[0])
                    role.LastAmbushed = DateTime.UtcNow;
                else if (interact[1])
                    role.LastAmbushed.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }

            return true;
        }
    }
}