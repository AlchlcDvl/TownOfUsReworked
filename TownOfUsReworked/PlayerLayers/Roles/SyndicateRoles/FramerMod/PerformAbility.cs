using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System;
using System.Collections.Generic;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.FramerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformAbility
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Framer))
                return false;

            if (!Utils.ButtonUsable(__instance))
                return false;

            var role = Role.GetRole<Framer>(PlayerControl.LocalPlayer);

            if (__instance == role.FrameButton)
            {
                if (role.FrameTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                if (!Role.SyndicateHasChaosDrive)
                {
                    var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence));

                    if (interact[3] == true && interact[0] == true)
                    {
                        role.Framed.Add(role.ClosestPlayer.PlayerId);
                        role.LastFramed = DateTime.UtcNow;
                    }
                    else if (interact[1] == true)
                        role.LastFramed.AddSeconds(CustomGameOptions.ProtectKCReset);
                }
                else
                {
                    var closestplayers = Utils.GetClosestPlayers(PlayerControl.LocalPlayer.GetTruePosition(), CustomGameOptions.ChaosDriveFrameRadius);

                    foreach (var player in closestplayers)
                    {
                        if (!player.Is(Faction.Syndicate))
                            role.Framed.Add(player.PlayerId);
                    }

                    role.LastFramed = DateTime.UtcNow;
                }

                return false;
            }
            else if (__instance == role.KillButton)
            {
                if (role.KillTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence), true);

                if (interact[3] == true || interact[0] == true)
                    role.LastKilled = DateTime.UtcNow;
                else if (interact[1] == true)
                    role.LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
                else if (interact[2] == true)
                    role.LastKilled.AddSeconds(CustomGameOptions.VestKCReset);
                
                return false;
            }

            return false;
        }
    }
}