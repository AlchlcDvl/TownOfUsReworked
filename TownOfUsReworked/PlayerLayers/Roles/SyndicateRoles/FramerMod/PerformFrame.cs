using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.FramerMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformFrame
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Framer))
                return true;

            var role = Role.GetRole<Framer>(PlayerControl.LocalPlayer);

            if (__instance == role.FrameButton)
            {
                if (role.FrameTimer() != 0f)
                    return false;

                if (!Role.SyndicateHasChaosDrive)
                {
                    if (Utils.IsTooFar(role.Player, role.ClosestFrame))
                        return false;

                    var interact = Utils.Interact(role.Player, role.ClosestFrame);

                    if (interact[3])
                        role.Frame(role.ClosestFrame);

                    if (interact[0])
                        role.LastFramed = DateTime.UtcNow;
                    else if (interact[1])
                        role.LastFramed.AddSeconds(CustomGameOptions.ProtectKCReset);
                }
                else
                {
                    foreach (var player in Utils.GetClosestPlayers(PlayerControl.LocalPlayer.GetTruePosition(), CustomGameOptions.ChaosDriveFrameRadius))
                        role.Frame(player);

                    role.LastFramed = DateTime.UtcNow;
                }

                return false;
            }

            return true;
        }
    }
}