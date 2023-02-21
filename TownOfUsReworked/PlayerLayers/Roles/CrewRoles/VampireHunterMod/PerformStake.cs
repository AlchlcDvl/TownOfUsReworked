using System;
using HarmonyLib;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Enums;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.VampireHunterMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public static class PerformStake
    {
        [HarmonyPriority(Priority.First)]
        private static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.VampireHunter))
                return false;

            var role = Role.GetRole<VampireHunter>(PlayerControl.LocalPlayer);

            if (__instance == role.StakeButton)
            {
                if (!Utils.ButtonUsable(__instance))
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                if (role.StakeTimer() != 0f)
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence), role.ClosestPlayer.Is(SubFaction.Undead));

                if (interact[3] == true && interact[0] == true)
                    role.LastStaked = DateTime.UtcNow;
                else if (interact[0] == true)
                    role.LastStaked = DateTime.UtcNow;
                else if (interact[1] == true)
                    role.LastStaked.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }

            return false;
        }
    }
}
