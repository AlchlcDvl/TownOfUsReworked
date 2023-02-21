using System;
using Reactor.Utilities;
using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.SeerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformSeer
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Seer))
                return false;

            var role = Role.GetRole<Seer>(PlayerControl.LocalPlayer);

            if (__instance == role.RevealButton)
            {
                if (!Utils.ButtonUsable(__instance))
                    return false;

                if (role.SeerTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;
                
                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence));

                if (interact[3] == true)
                {
                    var targetRoleCount = Role.GetRole(role.ClosestPlayer).RoleHistory.Count;

                    if (targetRoleCount > 0 || role.ClosestPlayer.IsFramed())
                        Coroutines.Start(Utils.FlashCoroutine(Color.red));
                    else
                        Coroutines.Start(Utils.FlashCoroutine(Color.green));
                }
                
                if (interact[0] == true)
                    role.LastRevealed = DateTime.UtcNow;
                else if (interact[1] == true)
                    role.LastRevealed.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }

            return false;
        }
    }
}
