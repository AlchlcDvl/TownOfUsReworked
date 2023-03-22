using System;
using Reactor.Utilities;
using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.SeerMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public class PerformSeer
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

                if (interact[3] == true)
                {
                    var targetRoleCount = Role.GetRole(role.ClosestPlayer).RoleHistory.Count;

                    if (targetRoleCount > 0 || role.ClosestPlayer.IsFramed())
                        Coroutines.Start(Utils.FlashCoroutine(Color.red));
                    else
                        Coroutines.Start(Utils.FlashCoroutine(Color.green));
                }
                
                if (interact[0] == true)
                    role.LastSeered = DateTime.UtcNow;
                else if (interact[1] == true)
                    role.LastSeered.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }

            return true;
        }
    }
}
