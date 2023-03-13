using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.FramerMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public class PerformFrame
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Framer))
                return true;

            if (!Utils.ButtonUsable(__instance))
                return false;

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

                    if (interact[3] == true)
                        role.Frame(role.ClosestFrame);

                    if (interact[0] == true)
                        role.LastFramed = DateTime.UtcNow;
                    else if (interact[1] == true)
                        role.LastFramed.AddSeconds(CustomGameOptions.ProtectKCReset);
                }
                else
                {
                    var closestplayers = Utils.GetClosestPlayers(PlayerControl.LocalPlayer.GetTruePosition(), CustomGameOptions.ChaosDriveFrameRadius);

                    foreach (var player in closestplayers)
                        role.Frame(player);

                    role.LastFramed = DateTime.UtcNow;
                }

                return false;
            }

            return true;
        }
    }
}