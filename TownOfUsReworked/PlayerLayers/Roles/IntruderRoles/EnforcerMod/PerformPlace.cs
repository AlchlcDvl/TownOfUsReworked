using System;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.EnforcerMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformPlace
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Enforcer))
                return true;

            var role = Role.GetRole<Enforcer>(PlayerControl.LocalPlayer);

            if (__instance == role.BombButton)
            {
                if (role.BombTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestBomb))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestBomb);

                if (interact[3])
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.SetBomb);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(role.ClosestBomb.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.TimeRemaining = CustomGameOptions.EnforceDuration;
                    role.TimeRemaining2 = CustomGameOptions.EnforceDelay;
                    role.BombedPlayer = role.ClosestBomb;
                    role.Delay();
                }
                else if (interact[0])
                    role.LastBombed = DateTime.UtcNow;
                else if (interact[1])
                    role.LastBombed.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }

            return true;
        }
    }
}