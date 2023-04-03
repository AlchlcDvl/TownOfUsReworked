using System;
using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.SeerMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public static class PerformSeer
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Seer))
                return true;

            var role = Role.GetRole<Seer>(PlayerControl.LocalPlayer);

            if (__instance == role.SeerButton)
            {
                if (role.SeerTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer);

                if (interact[3])
                {
                    var targetRoleCount = Role.GetRole(role.ClosestPlayer).RoleHistory.Count;

                    if (targetRoleCount > 0 || role.ClosestPlayer.IsFramed())
                        Utils.Flash(Color.red, $"{role.ClosestPlayer.Data.PlayerName} has changed their identity!");
                    else
                        Utils.Flash(Color.green, $"{role.ClosestPlayer.Data.PlayerName} has yet to change their identity!");
                }

                if (interact[0])
                    role.LastSeered = DateTime.UtcNow;
                else if (interact[1])
                    role.LastSeered.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }

            return true;
        }
    }
}