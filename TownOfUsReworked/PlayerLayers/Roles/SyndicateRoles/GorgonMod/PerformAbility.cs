using HarmonyLib;
using Hazel;
using TownOfUsReworked.Classes;
using System;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.GorgonMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformAbility
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Gorgon))
                return true;

            var role = Role.GetRole<Gorgon>(PlayerControl.LocalPlayer);

            if (__instance == role.GazeButton)
            {
                if (role.GazeTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, true);

                if (interact[3])
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.Gaze);
                    writer.Write(role.Player.PlayerId);
                    writer.Write(role.ClosestPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.Gazed.Add(role.ClosestPlayer.PlayerId);
                }

                if (interact[0])
                    role.LastGazed = DateTime.UtcNow;
                else if (interact[1])
                    role.LastGazed.AddSeconds(CustomGameOptions.ProtectKCReset);
                else if (interact[2])
                    role.LastGazed.AddSeconds(CustomGameOptions.VestKCReset);

                return false;
            }

            return true;
        }
    }
}